using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace csharpWeddings.Models
{
    public class Post : BaseEntity
    {
        [Key]
        public int PostId { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public User Creator { get; set; }

        public Wedding Wedding { get; set; }

        public int WeddingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Comment> Comments { get; set; }

        public Post()
        {
            Comments = new List<Comment>();
        }
    }
}