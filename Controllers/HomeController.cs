using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models.ViewModels;
using Blog.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }



        public IActionResult Index(string searchString, int? topicId)
        {
            ViewData["CurrentFilter"] = searchString;
            IndexVM vm = new IndexVM();

            vm.Topics = _db.Topics;

            if (!String.IsNullOrEmpty(searchString))
            {
                vm.Posts = _db.Posts.Include(x => x.Topics).Where(s => s.Title.Contains(searchString) || s.Name.Contains(searchString) || s.TopicId.ToString().Contains(searchString));
            }
            else if (topicId != null)
            {
                vm.Posts = _db.Posts.Include(x => x.Topics).Where(s => s.TopicId.ToString().Contains(topicId.ToString()));
            }
            else
            {
                vm.Posts = _db.Posts.Include(x => x.Topics);
            }

            //vm.Posts = !String.IsNullOrEmpty(searchString) ? _db.Posts.Include(x => x.Topics).Where(s => s.Title.Contains(searchString) || s.Name.Contains(searchString) || s.TopicId.ToString().Contains(searchString)) : _db.Posts.Include(x => x.Topics);
            
            
            return View(vm);
        }

        public IActionResult Post(int id)
        {
            PostVM vm = new PostVM()
            {
                Posts = _db.Posts.Include(x => x.Topics).FirstOrDefault(x => x.Id == id),
                ListOfPosts = _db.Posts,
                Topics = _db.Topics
            };
            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
