using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class AppUser : IdentityUser
    {
        [Display(Name = "Username")]
        public string Name { get; set; }

        [Display(Name = "Role")]
        public string RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual AppRole AppRole { get; set; }
    }
}
