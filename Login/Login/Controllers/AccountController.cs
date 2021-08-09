using Login.Filters;
using Login.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
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
        public IActionResult Login(string Name, string Surname, string Email, string Password, string IdentityNumber)
        {
            var user = _context.Users.FirstOrDefault(w => w.Name.Equals(Name) && w.Surname.Equals(Surname) && w.Email.Equals(Email) && w.Password.Equals(Password) && w.IdentityNumber.Equals(IdentityNumber));
            if (user != null)
            {
                HttpContext.Session.SetInt32("id", user.ID);
                HttpContext.Session.SetString("fullname", user.Name + "" + user.Surname);
                return Redirect("/Home/Index/");
            }
            return RedirectToAction("Index");
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
        public IActionResult LogOut()
        {

            HttpContext.Session.Clear();
            return RedirectToAction("Index");
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

