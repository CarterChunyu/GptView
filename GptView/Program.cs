using GptView.Access;
using GptView.Data;
using GptView.Middlewares;
using GptView.Helpers;
using GptView.Servicies;
using GptView.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);
IConfiguration config = builder.Configuration;

var encodeConn = config["Server:Status"] == "local" ?
           config.GetConnectionString("localDB") : config.GetConnectionString("remoteDB");
string connStr = "";
DesCrytoHelper.TryDesDecrypt
    (encodeConn, config["Des:k"], config["Des:iv"], out connStr);

// serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter())
    .WriteTo.File(formatter: new Serilog.Formatting.Json.JsonFormatter(),
        // 注意路徑中的斜線 / 會出現轉義字錯誤 
        path: @"Logs/log_.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.MSSqlServer(connectionString: connStr, sinkOptions: new MSSqlServerSinkOptions { TableName = "serverlogs" })
    .MinimumLevel.Information()
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    //Log.Information("登入成功");

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // serilog
    builder.Services.AddSerilog();

    // accessor
    builder.Services.AddHttpContextAccessor();

    // configure options
    builder.Services.Configure<Server>(builder.Configuration.GetSection("Server"));
    builder.Services.Configure<GoogleAuth>(builder.Configuration.GetSection("GoogleAuth"));

    // exceptionmiddleware
    builder.Services.AddTransient<GlobalExceptionHandlerMiddlerware>();

    // session
    builder.Services.AddSession(sessionOptions =>
    {
        sessionOptions.IdleTimeout = TimeSpan.FromMinutes(20);
        sessionOptions.IOTimeout = TimeSpan.FromMinutes(20);
    });
    builder.Services.AddDistributedMemoryCache();

    // dbcontext
    builder.Services.AddDbContext<GptDBContext>(optionbuilder =>
    {    
        // DB瞬時錯誤重試
        optionbuilder.UseSqlServer(connStr, options => options.EnableRetryOnFailure(
            maxRetryCount:5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
            ));
    });

    // httpclientfactory
    builder.Services.AddHttpClient
        ("oauth2", client => client.BaseAddress = new Uri("https://oauth2.googleapis.com/token"));
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


    // serilogmiddleware
    app.UseMiddleware<SerilogInfoMiddleware>();
    // exceptionmiddleware
    app.UseMiddleware<GlobalExceptionHandlerMiddlerware>();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=User}/{action=Index}/{id?}");

    app.Run();

    Log.Information("Server關閉");
}
catch(Exception ex)
{
    Log.Error(ex, "Server啟動異常");
}
finally
{
    Log.CloseAndFlush();
}