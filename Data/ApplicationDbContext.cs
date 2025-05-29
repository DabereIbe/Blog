using Blog.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Topics> Topics { get; set; }
        public DbSet<AppUser> User { get; set; }
        public DbSet<AppRole> Role { get; set; }
        public DbSet<Posts> Posts { get; set; }
    }
}
