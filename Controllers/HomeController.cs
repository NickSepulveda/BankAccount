using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LoginAndRegistration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace BankAccount.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
     
        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            _context = context;
        }
     
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("register")]     //associated route string (exclude the leading /)
            public IActionResult RegisterUser(User user)
            {
                if(ModelState.IsValid)
                {
                    // do somethng!  maybe insert into db?  then we will redirect
                    // If a User exists with provided email
                    if(_context.Users.Any(u => u.Email == user.Email))
                    {
                        // Manually add a ModelState error to the Email field, with provided
                        // error message
                        ModelState.AddModelError("Email", "Email already in use!");
                        return View("Index");
                        // You may consider returning to the View at this point
                    }
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    _context.Add(user);
                    user.Balance = 0;
                    _context.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    // Oh no!  We need to return a ViewResponse to preserve the ModelState, and the errors it now contains!
                    return View("Index");
                }
            }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("login/user")]
            public IActionResult LoginUser(LoginUser user)
            {
                if(ModelState.IsValid)
                {
                    var userInDb = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                    // If no user exists with provided email
                    if(userInDb == null)
                    {
                        // Add an error to ModelState and return to View!
                        ModelState.AddModelError("Email", "Email not in Database");
                        return View("Login");
                    }
                    // Initialize hasher object
                    var hasher = new PasswordHasher<LoginUser>();
                    // verify provided password against hash stored in db
                    var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.Password);
                    // result can be compared to 0 for failure
                    if(result == 0)
                    {
                        // handle failure (this should be similar to how "existing email" is handled)
                        ModelState.AddModelError("Password", "Invalid Password");
                        return View("Login");
                    }
                    ViewBag.UserFirstName = userInDb.FirstName;
                    ViewBag.UserLastName = userInDb.LastName;
                    return RedirectToAction("Success", userInDb);
                }
                else
                {
                    // Oh no!  We need to return a ViewResponse to preserve the ModelState, and the errors it now contains!
                    return View("Login");
                }
            }
        [HttpGet("account/{userid}")]
        public IActionResult Success(LoginUser user)
        {
            if(ModelState.IsValid)
            {
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Email not in Database");
                    return View();
                }
                List<Transaction> transactionsWithUser = _context.Transactions.Include(transaction => transaction.Creator).ToList();
                //transactionsWithUser.Reverse();
                ViewBag.UserTransactions = transactionsWithUser;
                ViewBag.UserId= userInDb.UserId;
                ViewBag.UserLastName = userInDb.LastName;
                ViewBag.UserFirstName = userInDb.FirstName;
                ViewBag.UserEmail = userInDb.Email;
                ViewBag.UserBalance = userInDb.Balance;
                ViewBag.Transactions = _context.Transactions.Where(l => l.UserId == userInDb.UserId).ToList();

                return View();
            }
            return View("Unauthorized");
        }
        [HttpPost("/new")]
        public IActionResult AddTransaction(Transaction newTransaction)
        {
            var userInDb = _context.Users.FirstOrDefault(u => u.UserId == newTransaction.UserId);
            if(ModelState.IsValid)
            {
                
                var example = newTransaction.Amount;
                if(newTransaction.Amount > 0)
                {
                    userInDb.Balance += newTransaction.Amount;
                }
                else if(newTransaction.Amount < 0 && newTransaction.Amount < userInDb.Balance)
                {
                    Console.WriteLine(userInDb.Balance + newTransaction.Amount);
                    var exam = userInDb.Balance + newTransaction.Amount;
                    if(exam >= 0)
                    {
                        userInDb.Balance = userInDb.Balance + newTransaction.Amount;
                    }
                    else 
                    {
                        ModelState.AddModelError("Amount", "Withdraw amount is greater than balance!");
                        List<Transaction> transactionsWithUser1 = _context.Transactions.Include(transaction => transaction.Creator).ToList();
                        ViewBag.UserTransactions = transactionsWithUser1;
                        ViewBag.UserId= userInDb.UserId;
                        ViewBag.UserLastName = userInDb.LastName;
                        ViewBag.UserFirstName = userInDb.FirstName;
                        ViewBag.UserEmail = userInDb.Email;
                        ViewBag.UserBalance = userInDb.Balance;
                        ViewBag.Transactions = _context.Transactions.Where(l => l.UserId == userInDb.UserId).ToList();
                        _context.SaveChanges();
                        return View("Success");
                    }
                }
                else
                {
                    ModelState.AddModelError("Amount", "Please Enter a Valid Number");
                }

                List<Transaction> transactionsWithUser = _context.Transactions.Include(transaction => transaction.Creator).ToList();
                ViewBag.UserTransactions = transactionsWithUser;
                ViewBag.UserId= userInDb.UserId;
                ViewBag.UserLastName = userInDb.LastName;
                ViewBag.UserFirstName = userInDb.FirstName;
                ViewBag.UserEmail = userInDb.Email;
                ViewBag.UserBalance = userInDb.Balance;
                ViewBag.Transactions = _context.Transactions.Where(l => l.UserId == userInDb.UserId).ToList();
                _context.Add(newTransaction);
                _context.SaveChanges();
                return RedirectToAction("Success", userInDb);
                }
            else
                {
                List<Transaction> transactionsWithUser = _context.Transactions.Include(transaction => transaction.Creator).ToList();
                ViewBag.UserTransactions = transactionsWithUser;
                ViewBag.UserId= userInDb.UserId;
                ViewBag.UserLastName = userInDb.LastName;
                ViewBag.UserFirstName = userInDb.FirstName;
                ViewBag.UserEmail = userInDb.Email;
                ViewBag.UserBalance = userInDb.Balance;
                ViewBag.Transactions = _context.Transactions.Where(l => l.UserId == userInDb.UserId).ToList();
                return View("Success");
                }
        }
    }
}
