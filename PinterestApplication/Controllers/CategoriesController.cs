using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PinterestApplication.Data;
using PinterestApplication.Data.Migrations;
using PinterestApplication.Models;

namespace PinterestApplication.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<Models.ApplicationUser> _userManager;

        public CategoriesController(ApplicationDbContext context)
        {
            db = context;
        }

        [Authorize(Roles="Admin, User")]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if(TempData.ContainsKey("Message"))
            {
                ViewBag.Message = TempData["Message"].ToString();
            }
            var categories = from category in db.Category
                             orderby category.Name 
                             select category;
            ViewBag.Categories = categories;
            return View();
        }

        [Authorize(Roles ="Admin, User")]
        public ActionResult New()
        {
            return View();
        }



        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public ActionResult New(Category category)
        {
            if(ModelState.IsValid)
            {
                db.Category.Add(category);
                db.SaveChanges();
                TempData["Message"] = "Category added successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(category);
            }
           
        }

        // [Authorize(Roles = "User,Admin")]
        [AllowAnonymous]
        public IActionResult Show(int id)
        {
            //SetAccessRights();

            var category = db.Category;

            if (category == null)
            {
                TempData["message"] = "Nu aveti drepturi";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Bookmarks");
            }

            return View(category);
        }



    }
}
