using System;
using System.Collections.Generic;

namespace csharpWeddings.Models
{
    public class Wrapper : BaseEntity
    {
        public List<User> Users { get; set; }
        public List<Wedding> Weddings { get; set; }
        public List<Guest> Guests { get; set; }

        public Wrapper(List<User> users, List<Wedding> weddings, List<Guest> guests)
        {
            Users = users;
            Weddings = weddings;
            Guests = guests;
        }
    }
}