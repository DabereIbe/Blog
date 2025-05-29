using Microsoft.AspNetCore.Mvc;
using Blog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Microsoft.EntityFrameworkCore;
using Blog.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Controllers
{
    [Authorize(Roles = "Admin, Writer")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public List<SelectListItem> GetTopics()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var topics = _db.Topics.ToList();
            foreach (var item in topics)
            {
                list.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });
            }
            return list;
        }

        public List<SelectListItem> GetRoles()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var roles = _db.Role.ToList();
            foreach (var item in roles)
            {
                list.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });
            }
            return list;
        }

        public IActionResult Topics()
        {
            var topics = _db.Topics.ToList();
            return View(topics);
        }

        [HttpGet]
        public IActionResult CreateTopic()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTopic(Topics model)
        {
            if (ModelState.IsValid)
            {
                _db.Topics.Add(model);
                _db.SaveChanges();
                return RedirectToAction("Topics");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult EditTopic(int? id)
        {
            var topic = _db.Topics.Find(id);
            return View(topic);
        }

        [HttpPost]
        public IActionResult EditTopic(Topics model)
        {
            if (ModelState.IsValid)
            {
                _db.Topics.Update(model);
                _db.SaveChanges();
                return RedirectToAction("Topics");
            }
            return View(model);
        }

        public IActionResult DeleteTopic(int? id)
        {
            var topic = _db.Topics.Find(id);
            _db.Topics.Remove(topic);
            _db.SaveChanges();
            return RedirectToAction("Topics");
        }

        public IActionResult Roles()
        {
            var roles = _db.Role.ToList();
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(AppRole model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid().ToString();
                await _roleManager.CreateAsync(model);
                return RedirectToAction("Roles");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(AppRole model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);
                await _roleManager.UpdateAsync(role);
                return RedirectToAction("Roles");
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            await _roleManager.DeleteAsync(role);
            return RedirectToAction("Roles");
        }

        public IActionResult Users()
        {
            var users = _db.User.ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.RolesList = GetRoles();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserVM model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser() { UserName = model.Name, Email = model.Email, Name = model.Name, RoleId = model.RoleId };
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                    return RedirectToAction("Users");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Users");
            }
            //var user = _db.User.Find(id);
            //_db.User.Remove(user);
            //_db.SaveChanges();
            return RedirectToAction("Users");
        }

        public IActionResult Posts()
        {
            var posts = _db.Posts.ToList();
            return View(posts);
        }

        [HttpGet]
        public IActionResult CreatePost()
        {
            ViewBag.TopicList = GetTopics();
            return View();
        }

        [HttpPost]
        public IActionResult CreatePost(Posts model, IFormFile file, string username)
        {
            if (ModelState.IsValid)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var uploadDir = webRootPath + @"\images\";
                var fileName = Path.GetFileName(file.FileName);
                //var extention = Path.GetExtension();
                using (var fileStream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                model.Image = fileName;
                model.DateCreated = DateTime.Now.ToLongDateString();
                model.Name = username;
                _db.Posts.Add(model);
                _db.SaveChanges();
                return RedirectToAction("Posts");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult EditPost(int? id)
        {
            ViewBag.TopicList = GetTopics();
            var post = _db.Posts.Find(id);
            return View(post);
        }

        [HttpPost]
        public IActionResult EditPost(Posts model, IFormFile file, string username)
        {
            if (ModelState.IsValid)
            {
                var objFromDB = _db.Posts.AsNoTracking().FirstOrDefault(x => x.Id == model.Id);

                var webRootPath = _webHostEnvironment.WebRootPath;
                var uploadDir = webRootPath + @"\images\";
                var fileName = Path.GetFileName(file.FileName);
                //var extention = Path.GetExtension();
                var oldFile = Path.Combine(uploadDir, objFromDB.Image);

                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }
                using (var fileStream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                model.Image = fileName;
                model.DateCreated = objFromDB.DateCreated;
                model.DateUpdated = DateTime.Now.ToLongDateString();
                model.Name = username;
                _db.Posts.Update(model);
                _db.SaveChanges();
                return RedirectToAction("Posts");
            }
            return View(model);
        }

        public IActionResult PublishPost(int id)
        {
            var post = _db.Posts.Find(id);
            post.Published = true;
            _db.SaveChanges();
            return RedirectToAction("Posts");
        }

        public IActionResult UnpublishPost(int id)
        {
            var post = _db.Posts.Find(id);
            post.Published = false;
            _db.SaveChanges();
            return RedirectToAction("Posts");
        }

        public IActionResult DeletePost(int id)
        {
            var post = _db.Posts.Find(id);
            var webRootPath = _webHostEnvironment.WebRootPath;
            var uploadDir = webRootPath + @"\images\";
            var oldFile = Path.Combine(uploadDir, post.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
            _db.Posts.Remove(post);
            _db.SaveChanges();
            return RedirectToAction("Posts");
        }
    }
}
