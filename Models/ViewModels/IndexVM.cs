using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models.ViewModels
{
    public class IndexVM
    {
        public IEnumerable<Topics> Topics { get; set; }
        public IEnumerable<Posts> Posts { get; set; }
    }
}
