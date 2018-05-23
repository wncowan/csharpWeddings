using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace csharpWeddings.Models
{
    public class Comment : BaseEntity
    {
        [Key]
        public int CommentId { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public User Creator { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}