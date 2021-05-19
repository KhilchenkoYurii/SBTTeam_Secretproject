using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PsychoWeb.Context;
using PsychoWeb.Models;
using PsychoWeb.ViewModels;

namespace PsychoWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UnitOfWork unitOfWork;
        private readonly ErrorMessage errorMessage;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager, AppDbContext appDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.unitOfWork = new UnitOfWork(appDbContext);
            this.errorMessage = new ErrorMessage();
        }

        /// <summary>
        /// Get method to show registration form.
        /// </summary>
        /// <returns>Register view that contains registration form.</returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Post method that will be runned after pushing on the button in registration form. 
        /// </summary>
        /// <param name="model">Model to unit all form fields in one entity.</param>
        /// <returns>If operation is success it redirects to main page, otherwise it will return the same page.</returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterView model)
        {
            if (ModelState.IsValid)
            {
                foreach (var number in model.Phone)
                {
                    if (!char.IsDigit(number))
                    {
                        ViewBag.Message = "Phone number is invalid!";
                        return View("ErrorPhonePage");
                    }
                }

                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email
                };

                var myUser = new Customer
                {
                    Name = model.FirstName,
                    Surname = model.SecondName,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    BirthDate = model.BirthDate
                    
                };

                if (userManager.Users.Where(u => u.Email == model.Email).Count() > 0)
                {
                    ViewBag.Message = "User with that email already exist";

                    return View("ErrorPage");
                }

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "customer");
                    await signInManager.SignInAsync(user, isPersistent: false);

                    await unitOfWork.Customers.Create(myUser);
                    await unitOfWork.SaveAsync();

                    return RedirectToAction("Index", "Home");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Get method to show login form.
        /// </summary>
        /// <returns>Login view that contains login form.</returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Method runs when user clicks on the submit button in Login view. 
        /// </summary>
        /// <param name="model">Model to unit all form fields in one entity.</param>
        /// <returns>If operation is success it redirects to main page, otherwise it will return the same page.</returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginView model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ViewBag.Message = "User is not registered";

                    return View("ErrorPage");
                }

                var result =
                    await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Wrong login or password!");
                }
            }

            return View(model);
        }

        /// <summary>
        /// Performs log out form site.
        /// </summary>
        /// <returns>Redirects to main page.</returns>
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CustomerSettingPage()
        {
            string name = User.Identity.Name;
            Customer customer = unitOfWork.Customers.GetAll().Where(c => c.Email == name)
                .FirstOrDefault();

            if (customer != null)
            {
                CustomerSettingView model = new CustomerSettingView
                {
                    Id = customer.CustomerId,
                    FirstName = customer.Name,
                    SecondName = customer.Surname,
                    Phone = customer.PhoneNumber,
                    Email = customer.Email,
                    BirthDate = customer.BirthDate
                };


                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> CustomerSettingPage(CustomerSettingView model)
        {
            Customer customer = await unitOfWork.Customers.Get(model.Id);

            if (customer != null)
            {
                foreach (var number in model.Phone)
                {
                    if (!char.IsDigit(number))
                    {
                        ViewBag.Message = "Phone number is invalid!";
                        return View("EditPhoneError");
                    }
                }

                customer.Name = model.FirstName;
                customer.Surname = model.SecondName;
                customer.PhoneNumber = model.Phone;
                
                ApplicationUser user = await userManager.FindByEmailAsync(model.Email);
                user.UserName = model.Email;

                unitOfWork.Customers.Update(customer);
                await unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index", "Home");
        }
       
        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
