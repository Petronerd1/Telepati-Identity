using Login.Filters;
using Login.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Login.Controllers
{
    public class AccountController : Controller
    {
        public readonly DataContext _context;
        private string code = null;
        public AccountController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("id").HasValue)
            {
                return RedirectToAction("/Home/Index/");

            }
            return View();
        }
        public IActionResult ForgotPassword()
        {
            if (HttpContext.Session.GetInt32("id").HasValue)
            {
                return RedirectToAction("/Home/Index/");

            }
            return View();
        }
            public IActionResult ResetPassword()
        {
            if (HttpContext.Session.GetInt32("id").HasValue)
            {
                return RedirectToAction("/Home/Index/");

            }
            return View();
        }
        public IActionResult SendCode(string email)
        {
            var user = _context.Users.FirstOrDefault(w => w.Email.Equals(email));
            if (user != null)
            {
                _context.Add(new PasswordCode { UserID = user.ID, Code = getCode() });
                _context.SaveChanges();
                string text = "Şİfrenizi sıfırlamak için kodunuz." + getCode() + "";
                string subject = "Parola Sıfırlama";
                MailMessage msg = new MailMessage("bedirhanttepe@gmail.com",email, subject, text);
                msg.IsBodyHtml = false;
                SmtpClient sc = new SmtpClient("smtp.gmail.com", 587);
                sc.UseDefaultCredentials = false;
                NetworkCredential cre = new NetworkCredential("bedirhanttepe@gmail.com", "Weareacmilan1899");
                sc.Credentials = cre;
                sc.EnableSsl = true;
                sc.Send(msg);
                return RedirectToAction("ResetPassword");

            }
            return RedirectToAction("Index");
        }
        public IActionResult ResetPasswordCode(string Code,string NewPassword)
        {
            var passwordcode = _context.PasswordCodes.FirstOrDefault(w => w.Code.Equals(Code));
            if(passwordcode!= null)
            {
                var user = _context.Users.Find(passwordcode.UserID);
                user.Password = NewPassword;
                _context.Update(user);
                _context.Remove(passwordcode);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Login(User p)
        {
            var user = _context.Users.FirstOrDefault(x => x.Name == p.Name && x.Surname == p.Surname && x.Email == p.Email && x.Password == p.Password && x.IdentityNumber == p.IdentityNumber);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,p.Name),
                    new Claim(ClaimTypes.Surname,p.Surname),
                    new Claim(ClaimTypes.Email,p.Email),


                };
                var useridentity = new ClaimsIdentity(claims,"Login");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                await HttpContext.SignInAsync(principal);
                HttpContext.Session.SetInt32("id", user.ID);
                HttpContext.Session.SetString("fullname", user.Name + "" + user.Surname);
                return RedirectToAction("Index","Home");
            }
            return View(); ;        
        }
        public IActionResult SignUp()
        {
            if (HttpContext.Session.GetInt32("id").HasValue)
            {
                return RedirectToAction("/Home/Index/");

            }
            return View();
        }
        public async Task<IActionResult> Register(User model)
        {
            await _context.AddAsync(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> LogOut()
        {
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Account");
        }
       
        
       
        public string getCode()
        {
            if (code == null)
            {
                Random rand = new Random();
                code = "";
                for (int i = 0; i < 6; i++)
                {
                    char tmp = Convert.ToChar(rand.Next(48, 58));
                    code += tmp;
                }
            }
            return code;
        }
        
    }
}

