using AutoMapper;
using Dapper;
using GptView.Data;
using GptView.Helpers;
using GptView.Models;
using GptView.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data.Common;
using System.Net.Http.Headers;

namespace GptView.Servicies
{
    public class UserService
    {
        private readonly GptDBContext _context;
        private readonly IConfiguration _config;
        private readonly IOptions<Server> _sOptions;
        private readonly IOptions<GoogleAuth> _gOptions;
        private readonly IHttpClientFactory _factory;
        private readonly IMapper _mapper;
        public UserService(GptDBContext context, IConfiguration config, IOptions<Server> sOptions
            , IOptions<GoogleAuth> gOptions, IHttpClientFactory factory, IMapper mapper)
        {
            _context = context;
            _config = config;
            _sOptions = sOptions;
            _gOptions = gOptions;
            _factory = factory;
            _mapper = mapper;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.email == email);
        }

        public async Task<User?> GetUserByAccount(string username, string password)
        {
            return await _context.Users.
                FirstOrDefaultAsync(x => x.username == username && x.password == password);
        }

        public async Task<int> InserUser(User user)
        {
            await _context.Users.AddAsync(user);
            return _context.SaveChanges();
        }

        public async Task<UserInfoVM> GetUserInfo(User user)
        {
            List<Function> functionList;
            DbConnection conn = _context.Database.GetDbConnection();
            functionList = (await conn.QueryAsync<Function>(
                @"select * from functions where functionid in 
(select functionid from accesspermissions where userid = @userid)", new { userid = user.userid })).ToList();

            return new UserInfoVM
            {
                userid = user.userid,
                username = !string.IsNullOrEmpty(user.nickname) ? user.nickname : user.email.Split("@")[0],
                functionlist = functionList
            };
        }

        public async Task<User?> GoogleLogin(string code)
        {
            DesCrytoHelper.TryDesDecrypt(_gOptions.Value.client_id,
                _config["Des:k"], _config["Des:iv"], out string client_id);
            DesCrytoHelper.TryDesDecrypt(_gOptions.Value.client_secret,
                _config["Des:k"], _config["Des:iv"], out string client_secret);
            var dicData = new Dictionary<string, string>()
            {
                {"client_id", client_id },
                {"client_secret", client_secret },
                {"code", code },
                {"grant_type", _gOptions.Value.grant_type },
                {"redirect_uri", _sOptions.Value.Status == "local"
                    ? _gOptions.Value.redirect_uri.local : _gOptions.Value.redirect_uri.remote },
                {"access_type", _gOptions.Value.access_type }
            };

            var tokenClient = _factory.CreateClient("oauth2");
            var response = await tokenClient.PostAsync("", new FormUrlEncodedContent(dicData));
            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<GoogleTokenResponse>(json);
            if (!tokenResponse.IsSuccess)
                return null;
            var userinfoClient = _factory.CreateClient("userinfo");
            userinfoClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenResponse.access_token);
            var userInfoResponse = await userinfoClient.GetAsync("");
            if (!userInfoResponse.IsSuccessStatusCode)
                return null;
            var infoJson = await userInfoResponse.Content.ReadAsStringAsync();
            var info = JsonConvert.DeserializeObject<GoogleUserInfo>(infoJson);

            var user = await GetUserByEmail(info?.email);
            if (user == null)
            {
                user = _mapper.Map<GoogleUserInfo, User>(info);
                if (await InserUser(user) != 1)
                    return user;
            }
            return user;
        }
    }
}
