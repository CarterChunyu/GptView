
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;

namespace GptView.Middlewares
{
    public class GlobalExceptionHandlerMiddlerware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlerMiddlerware> _logger;

        public GlobalExceptionHandlerMiddlerware(ILogger<GlobalExceptionHandlerMiddlerware> logger)
        {
            _logger = logger;   
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "出現異常");
                Regex regex = new Regex(@"/api/", RegexOptions.IgnoreCase);
                if (regex.IsMatch(context.Request.Path.Value))
                {                   
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync
                        (JsonConvert.SerializeObject(new { result = "Error", message = ex.Message }));
                }
                else
                {

                    context.Response.Redirect("/User/ExceptionPage?msg="+WebUtility.UrlEncode(ex.Message));
                }
            }
        }
    }
}
