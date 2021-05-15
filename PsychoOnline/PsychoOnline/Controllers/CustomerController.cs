using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PsychoOnline.Context;
using PsychoOnline.VIewModels;
using PsychoWeb.Context;
using PsychoWeb.Models;
using PsychoWeb.ViewModels;

namespace PsychoOnline.Controllers
{
    [Authorize(Roles = "admin")]
    public class CustomerController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UnitOfWork unitOfWork;
        private readonly ErrorMessage errorMessage;

        public CustomerController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager, AppDbContext appDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.unitOfWork = new UnitOfWork(appDbContext);
            this.errorMessage = new ErrorMessage();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var customers = unitOfWork.Customers.GetAll().ToList();
            return View(customers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerView model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.FirstName + model.SecondName,
                    CreateDate = DateTime.Now,
                    UpdateTime = DateTime.Now
                };

                Customer customer = new Customer
                {
                    Name = model.FirstName,
                    Surname = model.SecondName,
                    PhoneNumber = model.Phone,
                    Email = model.Email,
                    BirthDate = model.BirthDate
                };

                if (userManager.Users.Where(u => u.Email == model.Email).Count() > 0)
                {
                    ViewBag.Message = "User with that email already exist";

                    return View("ErrorPage");
                }

                var addResult = await userManager.CreateAsync(user, model.Password);

                if (addResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "customer");
                    await unitOfWork.Customers.Create(customer);
                    await unitOfWork.SaveAsync();

                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    foreach (var error in addResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Customer customer = await unitOfWork.Customers.Get(id);
            ApplicationUser user = await userManager.FindByEmailAsync(customer.Email);

            var userRoles = await userManager.GetRolesAsync(user);
            var allRoles = roleManager.Roles.ToList();

            if (customer == null)
            {
                return NotFound();
            }

            var model = new EditCustomerView
            {
                Id = customer.CustomerId,
                FirstName = customer.Name,
                SecondName = customer.Surname,
                Phone = customer.PhoneNumber,
                Email = customer.Email,
                AllRoles = allRoles,
                UserRoles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCustomerView model, List<string> roles)
        {
            if (ModelState.IsValid)
            {
                Customer customer = await unitOfWork.Customers.Get(model.Id);
                ApplicationUser user = await userManager.FindByEmailAsync(customer.Email);
                if (user != null && customer != null)
                {
                    if (!roles.Contains("manager") && model.MakePsychologist) 
                    {
                        roles.Add("manager");
                    }
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.UpdateTime = DateTime.Now;

                    var userRoles = await userManager.GetRolesAsync(user);
                    var allRoles = roleManager.Roles.ToList();

                    var addedRoles = roles.Except(userRoles);
                    var removedRoles = userRoles.Except(roles);

                    await userManager.AddToRolesAsync(user, addedRoles);
                    await userManager.RemoveFromRolesAsync(user, removedRoles);

                    if (model.MakePsychologist)
                    {
                        

                        var psychologist = new Psychologist
                        {
                            Name = model.FirstName,
                            Surname = model.SecondName,
                            BirthDate = customer.BirthDate,
                            Email = model.Email,
                            PhoneNumber = model.Phone
                        };

                        await unitOfWork.Psychologists.Create(psychologist);
                        await unitOfWork.Customers.Delete(customer.CustomerId);
                        await unitOfWork.SaveAsync();

                    }
                    else
                    {
                        customer.Name = model.FirstName;
                        customer.Surname = model.SecondName;
                        customer.PhoneNumber = model.Phone;
                        customer.Email = model.Email;

                        unitOfWork.Customers.Update(customer);
                        await unitOfWork.SaveAsync();

                        var result = await userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Index", "Customer");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            Customer customer = await unitOfWork.Customers.Get(id);
            ApplicationUser user = await userManager.FindByEmailAsync(customer.Email);

            if (user != null && customer != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                await unitOfWork.Customers.Delete(id);
                await unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index");
        }
        
        private bool AddToList(FindCustomerView model, Customer customer)
        {
            bool addToResult = true;

            if (model.FirstName == null && model.SecondName == null &&
            model.Phone == null && model.Email == null)
            {
                addToResult = false;
            }

            if (model.FirstName != null && customer.Name != model.FirstName)
            {
                addToResult = false;
            }

            if (model.SecondName != null && customer.Surname != model.SecondName)
            {
                addToResult = false;
            }

            if (model.Phone != null && customer.PhoneNumber != model.Phone)
            {
                addToResult = false;
            }

            if (model.Email != null && customer.Email != model.Email)
            {
                addToResult = false;
            }

            return addToResult;
        }
    }
}
