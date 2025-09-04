using Microsoft.AspNetCore.Mvc;

namespace MovieWeb.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult AboutUs()
        {
            return View("~/Views/AboutUs/AboutUs.cshtml");
        }
    }
}
