using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace csharpWeddings.Models
{
    public class Wedding : BaseEntity
    {
        public int Id { get; set; }
        public string WedderOne { get; set; }
        public string WedderTwo { get; set; }

        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public User User { get; set; }
        public int CreatorId { get; set; }
        public List<Guest> Guests { get; set; }

        public Wedding()
        {
            List<Guest> Guests = new List<Guest>();
        }
    }
}