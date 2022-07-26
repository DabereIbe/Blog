using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models.ViewModels
{
    public class PostVM
    {
        public Posts Posts { get; set; }
        public IEnumerable<Posts> ListOfPosts { get; set; }
        public IEnumerable<Topics> Topics { get; set; }
    }
}
