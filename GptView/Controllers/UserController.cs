using AutoMapper;
using GptView.Helpers;
using GptView.Models;
using GptView.Servicies;
using GptView.ViewModels;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GptView.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        private readonly UserService _userSerice;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IOptions<Server> _sOptions;
        private readonly IOptions<GoogleAuth> _gOptions;

        public UserController(UserService userService, IMapper mapper, IConfiguration config
            , IOptions<Server> sOptions, IOptions<GoogleAuth> gOptions)
        {
            _userSerice = userService;
            _mapper = mapper;
            _config = config;
            _sOptions = sOptions;
            _gOptions = gOptions;
        }

        [HttpGet]
        public IActionResult Index()
        {

            ViewBag.RedirectUrl = _sOptions.Value.Status == "local" ?
                _gOptions.Value.redirect_uri.local : _gOptions.Value.redirect_uri.remote;
            DesCrytoHelper.TryDesDecrypt(_gOptions.Value.client_id,
                _config["Des:k"], _config["Des:iv"], out string clintId);
            ViewBag.ClientId = clintId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserVM userVm)
        {
            if (userVm.submittype == SubmitType.login.ToString())
            {
                var user = await _userSerice.GetUserByAccount(userVm?.username, userVm?.password);
                if (user == null)
                {
                    ViewBag.ErrorMsg = "帳號密碼錯誤";
                    return View();
                }
                else
                {
                    // 登入成功
                    var userInfo = await _userSerice.GetUserInfo(user);
                    HttpContext.Session.Clear();
                    HttpContext.SetObjectToSession("UserInfo", userInfo);
                    return RedirectToAction("HomePage", "Home");
                }
            }
            else if (userVm.submittype != SubmitType.regiseter.ToString())
            {
                var user = await _userSerice.GetUserByEmail(userVm.email);
                if (user != null)
                {
                    ViewBag.ErrorMsg = "此信箱已經註冊過了";
                    return View();
                }
                else
                {
                    var userM = _mapper.Map<UserVM, User>(userVm);
                    var result = await _userSerice.InserUser(userM);
                    if (result == 1)
                        ViewBag.SuccessMsg = "帳號註冊成功";
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult NoPermission()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "User");
        }

        [HttpGet]
        public IActionResult ExceptionPage(string msg)
        {
            ViewBag.Msg = msg;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GoogleLogin()
        {
            var code = Request.Query["code"];
            var user = await _userSerice.GoogleLogin(code);
            if (user!= null)
            {
                var userinfo = await _userSerice.GetUserInfo(user);
                HttpContext.Session.Clear();
                HttpContext.SetObjectToSession("UserInfo", userinfo);
                return RedirectToAction("HomePage", "Home");
            }
            return RedirectToAction("NoPermission", "User");
        }
    }
}
