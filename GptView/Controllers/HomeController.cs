using GptView.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GptView.Controllers
{
    [Authorize(Policy ="MenuPolicy")]
    public class HomeController : Controller
    {      
        public IActionResult HomePage()
        {
            return View();
        }
    }
}
