using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Steema1.Context.Models;
using Steema1.Context;

namespace Сrypto_Monitor.Controllers
{
    [Route("Login")]
    public class LoginController : Controller
    {
        private PhysDataContext _context;
        public LoginController(PhysDataContext context) 
        {
            _context = context;
        }

        [HttpGet]
        [Route("Registration")]
        public IActionResult Registration()
        {
            return View("Registration");
        }

        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration([FromForm] User user)
        {
            if (ModelState.IsValid)
            {
                User us = _context.Users.FirstOrDefault(c => c.Email == user.Email);
                if (us is null)
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    us = _context.Users.FirstOrDefault(c => c.Email == user.Email);
                    await Authenticate(us.Email, us.Id.ToString());

                    return RedirectToAction("Index", "Home");
                }
            }
            return View("Registration");
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromForm] User user) 
        {
            if (ModelState.IsValid) 
            {
                User us = null;
                try
                {
                    us = _context.Users.FirstOrDefault(c => c.Email == user.Email);
                }
                catch (Exception e) 
                {
                    ViewBag.Message = e.Message;
                    return View("Login");
                }
                if (us != null)
                {
                    await Authenticate(us.Email, us.Id.ToString());
                    
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.Message = "not found user in database";
            }
            return View("Login");
        }

        public async Task Authenticate(string userEmail, string userId)
        {
            var claims = new List<Claim>
            {
                new Claim("Email", userEmail),
                new Claim("Id", userId)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }
    }
}
