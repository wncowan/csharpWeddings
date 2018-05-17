using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace csharpWeddings.Models
{
    public class Guest : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public Wedding Wedding { get; set; }
        public int WeddingId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }

    }
}