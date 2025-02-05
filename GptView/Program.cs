using GptView.Access;
using GptView.Data;
using GptView.ErrorMiddlewares;
using GptView.Helpers;
using GptView.Servicies;
using GptView.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// configure options
builder.Services.Configure<Server>(builder.Configuration.GetSection("Server"));
builder.Services.Configure<GoogleAuth>(builder.Configuration.GetSection("GoogleAuth"));

// exceptionmiddleware
builder.Services.AddTransient<GlobalExceptionHandlerMiddlerware>();

IConfiguration config = builder.Configuration;

// session
builder.Services.AddSession(sessionOptions =>
{
    sessionOptions.IdleTimeout = TimeSpan.FromMinutes(20);
    sessionOptions.IOTimeout = TimeSpan.FromMinutes(20);
});
builder.Services.AddDistributedMemoryCache();

// dbcontext
builder.Services.AddDbContext<GptDBContext>(options =>
{
    DesCrytoHelper.TryDesDecrypt
        (config.GetConnectionString("connStr"), config["Des:k"], config["Des:iv"], out string connStr);
    options.UseSqlServer(connStr);
});

// httpclientfactory
builder.Services.AddHttpClient
    ("oauth2", client => client.BaseAddress= new Uri("https://oauth2.googleapis.com/token"));
builder.Services.AddHttpClient
    ("userinfo", client => client.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v2/userinfo"));

// service
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<GraphService>();

// automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// httpcontextaccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, MenuAuthenticatioHandler>("MenuAuthScheme", options => { });

// policy
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
         //.RequireAuthenticatedUser()
         .AddRequirements(new MenuAccess())
         .AddAuthenticationSchemes("MenuAuthScheme")
         .Build();
    options.AddPolicy("MenuPolicy", policy);
});
builder.Services.AddSingleton<IAuthorizationHandler, MenuAccessHandler>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, MenuAccessMiddleware>();

// https://github.com/dotnet/aspnetcore/issues/50867
// 此種寫法會導致 IAuthorizationMiddlewareResultHandler 沒被呼叫到
//builder.Services.AddControllers(config => config.Filters.Add(new AuthorizeFilter("MenuPolicy")));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// session
app.UseSession();

app.UseAuthorization();

// exceptionmiddleware
app.UseMiddleware<GlobalExceptionHandlerMiddlerware>();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");

app.Run();
