using Microsoft.AspNetCore.Mvc;

namespace GptView.Controllers
{
    public class GptController : Controller
    {
        private readonly IConfiguration _config;
        public GptController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            ViewBag.url = _config["Url:Remote"];
            return View();
        }
    }
}
