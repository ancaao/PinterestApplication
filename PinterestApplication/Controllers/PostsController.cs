using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PinterestApplication.Data;
using PinterestApplication.Data.Migrations;
using PinterestApplication.Models;
using System.IO;
using ApplicationUser = PinterestApplication.Models.ApplicationUser;

namespace PinterestApplication.Controllers
{
    public class PostsController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(
                            ApplicationDbContext context,
                            UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "User, Admin")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var posts = db.Post.Include("Category").Include("User").Include("Likes").AsQueryable();
            /*                OrderByDescending(p => p.Date).OrderByDescending(p => p.Likes.Count);*/

            ViewBag.Posts = posts;

            var search = "";
            // MOTOR DE CAUTARE
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                // eliminam spatiile libere
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                // Căutare în titluri, cuvinte cheie și descrierea postărilor
                List<int> postIds = db.Post.Where
                    (p =>
                    p.Title.Contains(search) ||
                    p.Keywords.Contains(search) ||
                    p.Description.Contains(search))
                    .OrderBy(p => p.Title)
                    .Select(p => p.Id)
                    .ToList();

                // Lista postărilor care conțin cuvântul căutat
                posts = db.Post.Where(post => postIds.Contains(post.Id))
                    .Include("Category")
                    .Include("User")
                    .Include("Likes")
                    .OrderBy(p => p.Date);


                //ViewBag.Posts = posts;
            }

            string sortOrder = HttpContext.Request.Query["sortOrder"];
            if (sortOrder == "likesAsc")
            {
                posts = posts.OrderBy(p => p.Likes.Count);
            }
            else if (sortOrder == "likesDesc")
            {
                posts = posts.OrderByDescending(p => p.Likes.Count);
            }

            ViewBag.SearchString = search;

            int _perPage = 8;

            int totalItems = posts.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }
            var paginatedPosts =
            posts.Skip(offset).Take(_perPage);

            // Preluam numarul ultimei pagini
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            // Trimitem articolele cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.Posts = paginatedPosts;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Posts/Index/?search="
                + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Posts/Index/?page";
            }



            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }


        [Authorize(Roles = "User,Admin")]
        [AllowAnonymous]
        public IActionResult Show(int id)
        {
            Post post = db.Post
                   .Include("Category")
                   .Include("User")
                   .Include("Comments")
                   .Include("Comments.User")
                   .Include("Likes")
                   .Where(p => p.Id == id)
                   .First();

            if (post == null)
            {
                return NotFound(); // Tratează cazul în care nu s-a găsit un post cu id-ul specificat
            }

            var otherPosts = db.Post
                        .Where(p => p.CategoryId == post.CategoryId && p.Id != post.Id)
                        .OrderByDescending(p => p.Date)
                        .ToList();

            ViewBag.OtherPosts = otherPosts;

            // Adaugam bookmark-urile utilizatorului pentru dropdown
            ViewBag.UserBoards = db.Board
                                      .Where(b => b.UserId == _userManager.GetUserId(User))
                                      .ToList();


            SetAccessRights();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

           /* if (post == null)
            {
                return NotFound(); // Tratează cazul în care nu s-a găsit un post cu id-ul specificat
            }*/

            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]

        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comment.Add(comment);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comment.PostId);
            }
            else
            {
                Post posts = db.Post.Include("Category")
                                         .Include("User")
                                         .Include("Comments")
                                         .Include("Comments.User")
                                         .Where(posts => posts.Id == comment.PostId)
                                         .First();


                // Adaugam bookmark-urile utilizatorului pentru dropdown
                ViewBag.UserBoards = db.Board
                                          .Where(b => b.UserId == _userManager.GetUserId(User))
                                          .ToList();

                SetAccessRights();

                return View(posts);
            }
        }

        [HttpPost]
/*        [Authorize(Roles = "User,Admin")]*/
        public IActionResult AddBoard([FromForm] PostBoard postBoard)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Daca modelul este valid
                if (ModelState.IsValid)
                {
                    if (db.PostBoard
                        .Where(bm => bm.PostId == postBoard.PostId)
                        .Where(bm => bm.BoardId == postBoard.BoardId)
                        .Count() > 0)
                    {
                        TempData["message"] = "This post was already added to selectet board";
                        TempData["messageType"] = "alert-danger";
                    }
                    else
                    {
                        db.PostBoard.Add(postBoard);

                        db.SaveChanges();

                        TempData["message"] = "Post added to selected board successfully";
                        TempData["messageType"] = "alert-success";
                    }

                }
                else
                {
                    TempData["message"] = "Post could not be added to selected board";
                    TempData["messageType"] = "alert-danger";
                }
            }
            else
            {
                TempData["message"] = "You need to be logged in to add a post to a board";
                TempData["messageType"] = "alert-danger";
            }

            return Redirect("/Posts/Show/" + postBoard.PostId);
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
        public IActionResult New(Post post, IFormFile image)
        {
            post.Date = DateTime.Now;

            // preluam id-ul utilizatorului care posteaza bookmarkul
            post.UserId = _userManager.GetUserId(User);
            post.Keywords = post.Keywords?.Trim();

            if (image != null && image.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                image.CopyTo(memoryStream);
                post.Image = memoryStream.ToArray();
            }

            db.Post.Add(post);
            db.SaveChanges();
            TempData["message"] = "Post added";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Console.WriteLine(id);

            Post post= db.Post.Include("Category")
                                        .Where(p => p.Id == id)
                                        .First();

            Console.WriteLine(post);

            post.Categories = GetAllCategories();

            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(post);
            }

            else
            {
                TempData["message"] = "You are not allowed to edit this post";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Edit(int id, Post requestPost)
        {
            Post post = db.Post.Find(id);

            Console.WriteLine($"id:{id}");
            /*
                        if (post == null)
                        {
                            return NotFound();
                        }*/

            if (requestPost.Image == null)
            {
                ModelState.Remove("Image"); // Remove existing ModelState entry for Image
                requestPost.Image = null; // Set Image property to null
            }
            if (ModelState.IsValid)
            {
                Console.WriteLine("model state VALID");
                if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    post.Title = requestPost.Title;
                    post.Description = requestPost.Description;
                    post.CategoryId = requestPost.CategoryId;
                    post.Keywords = requestPost.Keywords;

                    TempData["message"] = "Post updated successfully";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    /*Console.WriteLine(post.Title);*/
                    return Redirect("/Posts/Show/" + post.Id);
                }
                else
                {
                    TempData["message"] = "You are not allowed to edit this post";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                requestPost.Categories = GetAllCategories();
                return View(requestPost);
            }
        }



        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Post post = db.Post.Include("Comments")
                                          .Include("Likes")
                                         .Where(bm => bm.Id == id)
                                         .First();

            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Post.Remove(post);
                db.SaveChanges();
                TempData["message"] = "Post deleted";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You are not allowed to delete this post";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.IsAdmin = User.IsInRole("Admin");

            ViewBag.CurrentUser = _userManager.GetUserId(User);
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

        public bool UserLikedPost(int postId)
        {   
            string userId = _userManager.GetUserId(User);
            return db.Like.Any(l => l.PostId == postId && l.UserId == userId);
        }

        [HttpPost]
        public async Task<IActionResult> AddLike(int id)
        {
            Post post= await db.Post.FindAsync(id);

            if (post != null)
            {
                string userId = _userManager.GetUserId(User);

                if(User.Identity.IsAuthenticated)
                {

                    if (UserLikedPost(post.Id))
                    {
                        // Utilizatorul a apreciat deja, retragem aprecierea
                        Like likeToRemove = db.Like.First(l => l.PostId == post.Id && l.UserId == userId);
                        db.Like.Remove(likeToRemove);
                    }
                    else
                    {
                        // Utilizatorul nu a apreciat încă, adăugăm aprecierea
                        Like like = new Like { PostId = post.Id, UserId = userId };
                        db.Like.Add(like);
                    }
                    await db.SaveChangesAsync();
                }
                else
                {
                    TempData["message"] = "You need to be logged in to like a post";
                    TempData["messageType"] = "alert-danger";
                }

            }
            return RedirectToAction("Show", new { id = id });
        }


        public IActionResult IndexNou()
        {
            return View();
        }
    }
}
