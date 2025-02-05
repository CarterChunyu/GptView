using GptView.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GptView.Controllers
{
    [Authorize(Policy ="MenuPolicy")]
    public class GptController : Controller
    {
        private readonly IOptions<Server> _options;
        private readonly IConfiguration _config;
        public GptController(IOptions<Server> options, IConfiguration config)
        {
            _options = options;
            _config = config;
        }
        public IActionResult Index()
        {
            ViewBag.url = _config[$"Url:{_options.Value.Status}"];
            return View();
        }
    }
}
