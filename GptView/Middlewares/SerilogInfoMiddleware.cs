using GptView.Helpers;
using GptView.ViewModels;
using Serilog.Context;

namespace GptView.Middlewares
{

    public class SerilogInfoMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _accessor;

        public SerilogInfoMiddleware(RequestDelegate next, IHttpContextAccessor accessor)
        {
            _next = next;
            _accessor = accessor;
        }

        public async Task Invoke(HttpContext context)
        {
            var userIp = context?.Connection.RemoteIpAddress?.MapToIPv4()?.ToString() ?? "Unknown IP";
            var userAccount = context?.GetObjectFromSession<UserInfoVM>("UserInfo")?.username ?? "UnKnown Username";
            var url = $"{context?.Request?.Scheme}://{context?.Request?.Host}{context?.Request?.Path}{context?.Request?.QueryString}";

            using (LogContext.PushProperty("IP", userIp))
            using (LogContext.PushProperty("Accout", userAccount))
            using (LogContext.PushProperty("Url", url))
            {
                await _next(context);
            }
        }
    }
}
