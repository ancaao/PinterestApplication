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
    public class BadgeController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public BadgeController(
                            ApplicationDbContext context,
                            UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var badges = db.Badge.ToList();
            return View();
        }

        //public IActionResult Show( int id)
        //{
        //    Badge badge = db.Badge
        //        .Include("Name")
        //        .Include("Description")
        //        .Include("ImagePath")
        //        .Where(b => b.Id == id)
        //        .First();

        //    if (badge == null)
        //    {
        //        return NotFound();
        //    }


        //    return View();
        //}
    }
}
