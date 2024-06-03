using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PinterestApplication.Data;
using PinterestApplication.Data.Migrations;
using PinterestApplication.Models;
using Category = PinterestApplication.Models.Category;
using ApplicationUser = PinterestApplication.Models.ApplicationUser;
using Microsoft.EntityFrameworkCore;

namespace PinterestApplication.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CategoriesController(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
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


        [Authorize(Roles = "Admin,User")]
        [AllowAnonymous]
        public IActionResult PostsByCategory(int categoryId)
        {
            var posts = db.Post.Include("Category")
                                    .Include("User")
                                    .Include("Likes")
                                    .Where(p => p.CategoryId == categoryId)
                                    .OrderByDescending(b => b.Date)
                                    .ToList();

            ViewBag.Post = posts;
            ViewBag.SelectedCategoryId = categoryId;

            return View("Index");
        }



        [Authorize(Roles ="Admin")]
        public ActionResult New()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "User,Admin")]
        [AllowAnonymous]
        public IActionResult Show(int id)
        {
            // SetAccessRights(); // Asigură-te că metoda SetAccessRights este implementată corect

            var category = db.Category
                .Include(c => c.Posts)
                .ThenInclude(p => p.Likes)
                .ThenInclude(p => p.User)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                TempData["message"] = "Category was not found";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Categories");
            }

            return View(category);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Category category = db.Category.Find(id);
            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Category requestCategory)
        {
            Category category = db.Category.Find(id);

            if (ModelState.IsValid)
            {

                category.Name = requestCategory.Name;
                db.SaveChanges();
                TempData["message"] = "Category modified";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestCategory);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Category category = db.Category.Find(id);
            db.Category.Remove(category);
            TempData["message"] = "Category deleted";
            db.SaveChanges();
            return RedirectToAction("Index");
        }



    }
}
