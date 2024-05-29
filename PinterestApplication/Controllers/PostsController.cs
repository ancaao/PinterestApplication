using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PinterestApplication.Data;
using PinterestApplication.Models;

namespace PinterestApplication.Controllers
{
    public class PostsController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "User, Admin")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var posts = db.Post.Include("Category").Include("User").
                OrderByDescending(p => p.Date);

            ViewBag.Posts = posts;
            return View();
        }


        [Authorize(Roles = "User,Admin")]
        [AllowAnonymous]
        public IActionResult Show(int id)
        {
            Post post = db.Post
                   .Include("Category")
                   .Include("User")
                   .Where(p => p.Id == id)
                   .First();

            // Adaugam bookmark-urile utilizatorului pentru dropdown
            /*            ViewBag.UserCollections = db.Collections
                                                  .Where(b => b.UserId == _userManager.GetUserId(User))
                                                  .ToList();*/


            //SetAccessRights();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            if (post == null)
            {
                return NotFound(); // Tratează cazul în care nu s-a găsit un post cu id-ul specificat
            }

            return View(post);
        }


        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Post post = new Post();

            // Se preia lista de categorii cu ajutorul metodei GetAllCategories()
            post.Categories = GetAllCategories();
            return View(post);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(Post post)
        {
            post.Date = DateTime.Now;

            // preluam id-ul utilizatorului care posteaza bookmarkul
            post.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Post.Add(post);
                db.SaveChanges();
                TempData["message"] = "Post added";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                post.Categories = GetAllCategories();
                return View(post);
            }
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Category
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name.ToString()
                });
            }

            // returnam lista de categorii
            return selectList;
        }
    }
}
