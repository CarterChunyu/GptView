using GptView.Helpers;
using GptView.Models;
using GptView.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GptView.ViewComponents
{
    public class FunctionList:ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userInfo = HttpContext.GetObjectFromSession<UserInfoVM>("UserInfo");
            userInfo.functionlist = userInfo.functionlist.Where(x => !x.isapi).ToList();
            await Task.CompletedTask;
            return View(userInfo);
        }
    }
}
