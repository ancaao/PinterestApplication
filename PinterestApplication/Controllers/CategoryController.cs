using Microsoft.AspNetCore.Mvc;

namespace PinterestApplication.Controllers
{
    public class CategoryController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }


    }
}
