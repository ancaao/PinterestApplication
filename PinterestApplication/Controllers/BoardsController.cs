using PinterestApplication.Data;
using PinterestApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PinterestApplication.Controllers
{
    [Authorize]
    public class BoardsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public BoardsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }


        // HttpGet - implicit
        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            SetAccessRights();

            if (User.IsInRole("User"))
            {
                var boards = db.Board.Include("User")
                               .Where(b => b.UserId == _userManager.GetUserId(User))
                                 .ToList();


                ViewBag.Boards = boards;
            }
            else if (User.IsInRole("Admin"))
            {
                var boards = from board in db.Board.Include("User")
                                  select board;

                ViewBag.Board = boards;
            }
            else
            {
                TempData["message"] = "You are not allowed to access this board";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Posts");
            }

            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id)
        {
            SetAccessRights();

            if (User.IsInRole("User"))
            {
                var boards = db.Board
                                  .Include("PostBoards.Post.Category")
                                  .Include("PostBoards.Post.User")
                                  .Include("User")
                                  .Where(b => b.Id == id)
                                  .FirstOrDefault();

                if (boards == null)
                {
                    TempData["message"] = "The requested board could not be found.";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Posts");
                }

                return View(boards);
            }
            else if (User.IsInRole("Admin"))
            {
/*                var boards = db.Board
                                  .Include("PostBoard.Post.Category")
                                  .Include("PostBoard.Post.User")
                                  .Include("User")
                                  .Where(b => b.Id == id)
                                  .FirstOrDefault();
*/
                var boards = db.Board
                                .Include(b => b.PostBoards)
                                    .ThenInclude(pb => pb.Post)
                                        .ThenInclude(p => p.Category)
                                .Include(b => b.PostBoards)
                                    .ThenInclude(pb => pb.Post)
                                        .ThenInclude(p => p.User)
                                .Include(b => b.User)
                                .FirstOrDefault(b => b.Id == id);


                if (boards == null)
                {
                    TempData["message"] = "What you are searching for does not exist";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Posts");
                }
                return View(boards);
            }
            else
            {
                TempData["message"] = "You are not allowed to access this board";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Posts");
            }
        }


        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(Board b)
        {
            b.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Board.Add(b);
                db.SaveChanges();
                TempData["message"] = "Board added";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                return View(b);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public ActionResult Delete(int id)
        {
            Board board = db.Board.Find(id);

            if (board == null)
            {
                TempData["message"] = "Board cannot be found";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            db.Board.Remove(board);
            TempData["message"] = "Board deleted";
            TempData["messageType"] = "alert-success";
            db.SaveChanges();
            return RedirectToAction("Index");
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






        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult DeleteFromBoard(int boardId, int postId)
        {
            Board board= db.Board
                .Include(b => b.PostBoards)
                .ThenInclude(pb => pb.Post)
                .FirstOrDefault(b => b.Id == boardId);

            if (board == null)
            {
                return NotFound();
            }

            var postBoardToRemove= board.PostBoards.FirstOrDefault(pb => pb.PostId == postId);

            if (postBoardToRemove != null)
            {
                // Verificați dacă utilizatorul curent are permisiunea să șteargă această asociere
                if (User.IsInRole("Admin") || board.UserId == _userManager.GetUserId(User))
                {
                    board.PostBoards.Remove(postBoardToRemove);
                    db.SaveChanges();

                    var post= db.Post
                        .Include(p => p.PostBoards)
                        .FirstOrDefault(p => p.Id == postId);

                    if (post != null && !post.PostBoards.Any())
                    {
                        post.PostBoards.Remove(postBoardToRemove);
                        db.SaveChanges();
                    }

                    TempData["message"] = "Post removed from board";
                    TempData["messageType"] = "alert-success";
/*                    return RedirectToAction("Show", new { id = boardId });*/
                }
                else
                {
                    TempData["message"] = "You are not allowed to remove the post from the board";
                    TempData["messageType"] = "alert-danger";
/*                    return RedirectToAction("Show", new { id = boardId });*/
                }
            }
            else
            {
                TempData["message"] = "The post is not in this board";
                TempData["messageType"] = "alert-danger";
            }

            return RedirectToAction("Show", new { id = boardId });
        }

    }
}