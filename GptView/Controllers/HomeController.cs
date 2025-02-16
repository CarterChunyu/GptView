using GptView.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GptView.Controllers
{
    [Authorize(Policy ="MenuPolicy")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;   
        }

        public IActionResult HomePage()
        {
            _logger.LogInformation("登入成功");
            return View();
        }
    }
}
