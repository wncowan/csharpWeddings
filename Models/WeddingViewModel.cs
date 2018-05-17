using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace csharpWeddings.Models
{
    public class WeddingViewModel : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage="You must enter a name")]
        public string WedderOne { get; set; }

        [Required(ErrorMessage="You must enter a name")]
        public string WedderTwo { get; set; }

        [Required(ErrorMessage="You must enter a date")]

        public DateTime Date { get; set; }

        [Required(ErrorMessage="You must enter an address")]
        public string Address { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public List<Guest> Guests { get; set; }
    }
}