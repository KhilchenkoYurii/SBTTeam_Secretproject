using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PsychoOnline.Context;
using PsychoOnline.VIewModels;
using PsychoWeb.Context;
using PsychoWeb.Models;

namespace PsychoOnline.Controllers
{
    [Authorize(Roles = "admin")]
    public class PsychologistController:Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UnitOfWork unitOfWork;
        private readonly ErrorMessage errorMessage;

        public PsychologistController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
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
            var psychologists = unitOfWork.Psychologists.GetAll().ToList();
            return View(psychologists);
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

                Psychologist customer = new Psychologist
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
                    await unitOfWork.Psychologists.Create(customer);
                    await unitOfWork.SaveAsync();

                    return RedirectToAction("Index", "Psychologist");
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
            Psychologist customer = await unitOfWork.Psychologists.Get(id);
            ApplicationUser user = await userManager.FindByEmailAsync(customer.Email);

            var userRoles = await userManager.GetRolesAsync(user);
            var allRoles = roleManager.Roles.ToList();

            if (customer == null)
            {
                return NotFound();
            }

            var model = new EditPsychologistView
            {
                Id = customer.PsychologistId,
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
        public async Task<IActionResult> Edit(EditPsychologistView model, List<string> roles)
        {
            if (ModelState.IsValid)
            {
                Psychologist psychologist = await unitOfWork.Psychologists.Get(model.Id);
                ApplicationUser user = await userManager.FindByEmailAsync(psychologist.Email);
                if (user != null && psychologist != null)
                {
                    if (roles.Contains("manager") && model.UnMakePsychologist)
                    {
                        roles.Remove("manager");
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

                    if (model.UnMakePsychologist)
                    {
                        var customer = new Customer
                        {
                            Name = model.FirstName,
                            Surname = model.SecondName,
                            BirthDate = psychologist.BirthDate,
                            Email = model.Email,
                            PhoneNumber = model.Phone
                        };

                        await unitOfWork.Customers.Create(customer);
                        await unitOfWork.Psychologists.Delete(psychologist.PsychologistId);
                        await unitOfWork.SaveAsync();

                    }
                    else
                    {
                        psychologist.Name = model.FirstName;
                        psychologist.Surname = model.SecondName;
                        psychologist.PhoneNumber = model.Phone;
                        psychologist.Email = model.Email;

                        unitOfWork.Psychologists.Update(psychologist);
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

            return RedirectToAction("Index", "Psychologist");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            Psychologist customer = await unitOfWork.Psychologists.Get(id);
            ApplicationUser user = await userManager.FindByEmailAsync(customer.Email);

            if (user != null && customer != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                await unitOfWork.Psychologists.Delete(id);
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

