using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Domain
{
    public class Blog
    {
        [Key]
        public Guid BlogId { get; set; }
        public string Url { get; set; }
        public virtual IQueryable<Post> Posts { get; set; }
    }
}
