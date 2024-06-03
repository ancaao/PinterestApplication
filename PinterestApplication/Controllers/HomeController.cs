using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PinterestApplication.Models;
using System.Diagnostics;

namespace PinterestApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.UserId = userId;

            /*            return View();*/
            return RedirectToAction("Index", "Posts");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
