using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Session;
using csharpWeddings.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace csharpWeddings.Controllers
{
    public class HomeController : Controller
    {

        private WeddingContext _context;

        public HomeController(WeddingContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User CheckUser = _context.Users.SingleOrDefault(u => u.Email == model.Email);
                Console.WriteLine(CheckUser);
                if(CheckUser != null)
                {
                    TempData["EmailInUseError"] = "Email Aleady in use";
                    return RedirectToAction("Index");
                }
                User newUser = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Wins = 0,
                    Losses = 0,
                    Created_At = DateTime.Now,
                    Updated_At = DateTime.Now
                };
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                newUser.Password = hasher.HashPassword(newUser, model.Password);
                _context.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("currentUserId", newUser.UserId);
                HttpContext.Session.SetString("currentFirstName", newUser.Username);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult Login(string email, string password)
        {
            User logUser = _context.Users.SingleOrDefault(user => user.Email == email);
            if (logUser == null)
            {
                TempData["EmailError"] = "Invalid email!";
                return RedirectToAction("Index");
            }
            else
            {
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                if (hasher.VerifyHashedPassword(logUser, logUser.Password, password) != 0)
                {
                    HttpContext.Session.SetInt32("currentUserId", logUser.UserId);
                    HttpContext.Session.SetString("currentUserEmail", logUser.Email);
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["PasswordError"] = "Invalid password";
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpGet]
        [Route("/dashboard")]
        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                List<User> users = _context.Users.Include(u => u.Weddings).ToList();
                List<Wedding> weddings = _context.Weddings.Include(w => w.Guests).Include(w => w.User).ToList();
                //maybe an error but has no effect
                List<Guest> guests = _context.Guests.Include(g => g.Wedding).ThenInclude(w => w.User).ToList();

                ViewBag.User = currentUser;
                ViewBag.Weddings = weddings;
                
                Wrapper model = new Wrapper(users, weddings, guests);
                return View(model);
            }
        }

        [HttpGet]
        [Route("/weddings/create")]
        public IActionResult NewWedding()
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;

                List<User> users = _context.Users.ToList();
                Console.WriteLine(users);
                List<Wedding> weddings = _context.Weddings.ToList();
                List<Guest> guests = _context.Guests.ToList();

                ViewBag.Users = users;

                return View();
            }
        }

        [HttpPost]
        [Route("/addWedding")]
        public IActionResult CreateWedding(WeddingViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                if (ModelState.IsValid)
                {
                    Wedding newWedding = new Wedding
                    {
                        WedderOne = model.WedderOne,
                        WedderTwo = model.WedderTwo,
                        Address = model.Address,
                        CreatorId = currentUser.UserId,
                        User = currentUser
                    };
                    if (model.Date < DateTime.Now)
                    {
                        TempData["DateError"] = "Dates cannot be in the past";
                        return RedirectToAction("NewWedding");
                    }
                    else 
                    {
                        newWedding.Date = model.Date;
                        _context.Add(newWedding);
                        _context.SaveChanges();
                        return RedirectToAction("Dashboard");
                    }
                }
                return RedirectToAction("NewWedding");
            }   
        }

        [HttpGet]
        [Route("/weddings/{weddingId}")]
        public IActionResult Wedding(int weddingId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;

                Wedding thisWedding = _context.Weddings.SingleOrDefault(w => w.Id == (int)weddingId);
                List<Guest> guests = _context.Guests.Where(g => g.WeddingId == (int)weddingId).Include(g => g.User).ToList();
                // List<User> allUsers = _context.Users.ToList();

                // ViewBag.Users = allUsers;
                User winner = _context.Users.SingleOrDefault(u => u.UserId == thisWedding.WinnerId);
                User loser = _context.Users.SingleOrDefault(u => u.UserId == thisWedding.LoserId);
                List<Post> posts = _context.Posts.Where(p => p.WeddingId == (int)weddingId).Include(p => p.Creator).Include(p => p.Comments).ToList();

                ViewBag.Winner = winner;
                ViewBag.Loser = loser;
                ViewBag.Posts = posts;
                ViewBag.Wedding = thisWedding;
                ViewBag.Guests = guests;
                
                return View();
            }
        }

        [HttpGet]
        [Route("/users/{userid}")]
        public IActionResult ShowUser(int userid)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;

                User showUser = _context.Users.Include(u => u.Posts).Include(u => u.Comments).SingleOrDefault(u => u.UserId == userid);
                ViewBag.ShowUser = showUser;

                // Wedding thisWedding = _context.Weddings.SingleOrDefault(w => w.Id == (int)weddingId);
                // List<Guest> guests = _context.Guests.Where(g => g.WeddingId == (int)weddingId).Include(g => g.User).ToList();
                // List<User> allUsers = _context.Users.ToList();

                // ViewBag.Users = allUsers;
                // User winner = _context.Users.SingleOrDefault(u => u.UserId == thisWedding.WinnerId);
                // User loser = _context.Users.SingleOrDefault(u => u.UserId == thisWedding.LoserId);

                // ViewBag.Winner = winner;
                // ViewBag.Loser = loser;
                return View();
            }
        }

        [HttpGet]
        [Route("/weddings/{weddingId}/rsvp")]
        public IActionResult RSVP(int weddingId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                Wedding thisWedding = _context.Weddings.SingleOrDefault(w => w.Id == (int)weddingId);

                Guest newGuest = new Guest
                {
                    UserId = currentUser.UserId,
                    WeddingId = thisWedding.Id,
                    Wedding = thisWedding,
                    User = currentUser
                };
                _context.Add(newGuest);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        [Route("/weddings/{weddingId}/simulate")]
        public IActionResult Simulate(int weddingId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                Wedding thisWedding = _context.Weddings.SingleOrDefault(w => w.Id == (int)weddingId);

                User p1 = _context.Users.SingleOrDefault(u => u.Username == thisWedding.WedderOne);
                User p2 = _context.Users.SingleOrDefault(u => u.Username == thisWedding.WedderTwo);
                Random rand = new Random();
                if(rand.Next(1,100) - p1.UserId < rand.Next(1,100) - p2.UserId)
                {
                    thisWedding.WinnerId = p1.UserId;
                    thisWedding.LoserId = p2.UserId;
                    p1.Wins += 1;
                    p2.Losses += 1;
                    
                }
                else
                {
                    thisWedding.WinnerId = p2.UserId;
                    thisWedding.LoserId = p1.UserId;
                    p2.Wins += 1;
                    p1.Losses += 1;
                    
                }
                
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

// @if(wedding.CreatorId == @ViewBag.User.UserId)
//             {
//                 <td><a href="/delete">Delete</a></td>
//             }
//             else
//             {
//                 <td><a href="/weddings/@wedding.Id/rsvp">RSVP</a></td>
//             }

        [HttpGet]
        [Route("/weddings/{weddingId}/leave")]
        public IActionResult Leave(int weddingId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                // Wedding thisWedding = _context.Weddings.SingleOrDefault(w => w.Id == (int)weddingId);
                // Guest thisGuest = _context.Guests.SingleOrDefault(g => g.Id == (int)guestId);
                Guest thisGuest = _context.Guests.Where(g => g.UserId == (int)userId).Where(wedding => wedding.WeddingId == weddingId).SingleOrDefault();
                

                _context.Remove(thisGuest);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        [Route("/weddings/{weddingId}/delete")]
        public IActionResult Delete(int weddingId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                Wedding thisWedding = _context.Weddings.SingleOrDefault(w => w.Id == (int)weddingId);
                _context.Remove(thisWedding);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        [Route("/weddings/{weddingId}/post")]
        public IActionResult Post(Post model, int weddingId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                Wedding thisWedding = _context.Weddings.SingleOrDefault(w => w.Id == (int)weddingId);
                
                if(ModelState.IsValid)
                {
                    Post newPost = new Post
                    {
                        Content = model.Content,
                        UserId = currentUser.UserId,
                        Creator = currentUser,
                        WeddingId = thisWedding.Id,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now 
                    };
                    _context.Add(newPost);
                    _context.SaveChanges();
                }

                
                return RedirectToAction("Wedding");
            }
        }

        [HttpPost]
        [Route("/weddings/{weddingId}/{postId}/comment")]
        public IActionResult Comment(Comment model, int postId, int weddingId)
        {
            int? userId = HttpContext.Session.GetInt32("currentUserId");
            if (userId == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return RedirectToAction("Index");
            }
            else
            {
                User currentUser = _context.Users.SingleOrDefault(u => u.UserId == (int)userId);
                ViewBag.User = currentUser;
                Post thisPost = _context.Posts.SingleOrDefault(w => w.PostId == (int)postId);
                
                if(ModelState.IsValid)
                {
                    Comment newComment = new Comment
                    {
                        Content = model.Content,
                        UserId = currentUser.UserId,
                        Creator = currentUser,
                        PostId = thisPost.PostId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now 
                    };
                    _context.Add(newComment);
                    _context.SaveChanges();
                }

                
                return RedirectToAction("Wedding");
            }
        }

        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}