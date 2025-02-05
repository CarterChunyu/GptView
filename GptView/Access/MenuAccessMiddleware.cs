using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Newtonsoft.Json;

namespace GptView.Access
{
    public class MenuAccessMiddleware : IAuthorizationMiddlewareResultHandler
    {
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Succeeded)
                await next.Invoke(context);
            else
            {
                if (context.Request.Path.Value.ToLower().Contains("api"))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(
                        JsonConvert.SerializeObject(new { result = "fail", message = "Login Timeout" }));
                }
                else
                    context.Response.Redirect("/User/NoPermission");
            }
        }
    }
}
