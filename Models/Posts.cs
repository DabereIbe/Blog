using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Posts
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Topic")]
        public int TopicId { get; set; }

        public string Name { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public string Image { get; set; }

        public bool Published { get; set; }

        public string DateCreated { get; set; }

        public string DateUpdated { get; set; }

        [ForeignKey("TopicId")]
        public virtual Topics Topics { get; set; }

        
    }
}
