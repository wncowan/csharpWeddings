using System;
using System.Collections.Generic;

namespace csharpWeddings.Models
{
    public class User : BaseEntity
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        // public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public List<Wedding> Weddings { get; set; }
        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }




        public User()
        {
            List<Wedding> Weddings = new List<Wedding>();
            List<Post> Posts = new List<Post>();
            List<Comment> Comments = new List<Comment>();

         }
    }
}

// else if (@wedding.Guests.UserId == @ViewBag.User.UserId) 
// {
//     <td><a href="/weddings/@wedding.Id/leave">Un-RSVP</a></td>
// }