using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Domain
{
    public class Post
    {
        [Key]
    
        public Guid PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
       
        [ForeignKey("Blog")]
        public Guid BlogId { get; set; }
        public virtual Blog Blog { get; set; }
    }
}
