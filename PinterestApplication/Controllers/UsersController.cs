using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PinterestApplication.Data;
using PinterestApplication.Models;

namespace PinterestApplication.Controllers
{

    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        [Authorize(Roles = "Admin,User,Editor")]
        [AllowAnonymous]
        [HttpGet("Users/Profile/{userId:guid}")]
        public IActionResult Profile(string userId)
        {
            var user = db.Users
                          .Include(u => u.Boards)
                          .Include(u => u.Posts)
                          .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                // Utilizatorul nu a fost găsit, puteți redirecta sau afișa o eroare
                return NotFound();
            }

            return View(user);
        }
        [Authorize(Roles = "Admin,User,Editor")]
        public IActionResult Profile()
        {
            var userId = _userManager.GetUserId(User);
            var user = db.Users
                          .Include(u => u.Boards)
                          .Include(u => u.Posts)
                          .Include(u => u.Badges)
                          .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                // Utilizatorul nu a fost găsit, puteți redirecta sau afișa o eroare
                return NotFound();
            }

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;

            ViewBag.UsersList = users;

            return View();
        }


        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Show(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                TempData["message"] = "The user could not be found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            return View(user);
        }

        [Authorize(Roles = "Admin, User,Editor")]
        public async Task<ActionResult> Edit(string id)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (User.IsInRole("Admin") || User.IsInRole("Editor") || id == currentUserId)
            {
                ApplicationUser user = await db.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                user.AllRoles = GetAllRoles();

                var roleNames = await _userManager.GetRolesAsync(user);
                var currentUserRole = _roleManager.Roles
                                                  .Where(r => roleNames.Contains(r.Name))
                                                  .Select(r => r.Id)
                                                  .FirstOrDefault();

                ViewBag.UserRole = currentUserRole;
                return View(user);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User,Editor")]
        public async Task<ActionResult> Edit(string id, ApplicationUser newData, [FromForm] string newRole)
        {
            var currentUserId = _userManager.GetUserId(User);
            ApplicationUser user = await db.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Admin") || id == currentUserId)
            {
                if (ModelState.IsValid)
                {
                    user.UserName = newData.UserName;
                    user.Email = newData.Email;
                    user.FirstName = newData.FirstName;
                    user.LastName = newData.LastName;
                    //set the user's role to its current role
                    //not from new data because that is what comes from user input and we dont have that
                    
                    //user.PhoneNumber = newData.PhoneNumber;

                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                        var roles = db.Roles.ToList();

                        foreach (var role in roles)
                        {
                            await _userManager.RemoveFromRoleAsync(user, role.Name);
                        }

                        var roleName = await _roleManager.FindByIdAsync(newRole);
                        if (roleName != null)
                        {
                            await _userManager.AddToRoleAsync(user, roleName.Name);
                        }
                    

                    TempData["message"] = "User updated successfully.";
                    TempData["messageType"] = "alert-success";
                    //return RedirectToAction("Index");
                    return RedirectToAction("Profile", new { id = user.Id });
                }
                else
                {
                    TempData["message"] = "Invalid data.";
                    TempData["messageType"] = "alert-danger";
                    user.AllRoles = GetAllRoles();
                    ViewBag.UserRole = newRole;
                    return View(user);

                }
            }

            else 
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User,Editor")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["message"] = "Invalid user ID.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            var user = await db.Users
                               .Include(u => u.Posts)
                               .Include(u => u.Comments)
                               .Include(u => u.Boards)
                               .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                TempData["message"] = "User not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            // Delete user comments
            if (user.Comments != null && user.Comments.Count > 0)
            {
                db.Comment.RemoveRange(user.Comments);
            }

            // Delete user posts
            if (user.Posts != null && user.Posts.Count > 0)
            {
                db.Post.RemoveRange(user.Posts);
            }

            // Delete user boards
            if (user.Boards != null && user.Boards.Count > 0)
            {
                db.Board.RemoveRange(user.Boards);
            }

            db.Users.Remove(user);

            try
            {
                await db.SaveChangesAsync();
                TempData["message"] = "User deleted successfully.";
                TempData["messageType"] = "alert-success";
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error deleting user: " + ex.Message;
                TempData["messageType"] = "alert-danger";
            }

            return RedirectToAction("Index");
        }
    


[NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
    }
}