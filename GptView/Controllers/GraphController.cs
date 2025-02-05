using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GptView.Controllers
{
    [Authorize(Policy = "MenuPolicy")]
    public class GraphController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
