using GptView.Helpers;
using GptView.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace GptView.Access
{
    public class MenuAccessHandler : AuthorizationHandler<MenuAccess>
    {
        private readonly IHttpContextAccessor _accessor;

        public MenuAccessHandler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MenuAccess requirement)
        {
            try
            {
                var urlArr = _accessor.HttpContext.Request.Path.Value.Split("/");
                var requestFunctions = urlArr[1].ToLower() == "api" ?
                    $"/{urlArr[2]}/{urlArr[3]}" : $"/{urlArr[1]}/{urlArr[2]}";
                var userInfoVm = _accessor.HttpContext.GetObjectFromSession<UserInfoVM>("UserInfo");

                if (userInfoVm.functionlist?.Any(x => x.functionpath == requestFunctions)==true)
                    context.Succeed(requirement);
                else
                    context.Fail();
            }
            catch(Exception ex)
            {
                context.Fail();
            }
            await Task.CompletedTask;
        }
    }
}
