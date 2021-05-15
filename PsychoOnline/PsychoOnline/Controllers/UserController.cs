using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using PsychoOnline.Context;
using PsychoOnline.VIewModels;
using PsychoWeb.Context;
using PsychoWeb.Models;

namespace PsychoOnline.Controllers
{
    [Authorize(Roles = "customer")]
    public class UserController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public UserController(AppDbContext appDbContext)
        {
            this.unitOfWork = new UnitOfWork(appDbContext);
        }

        public async Task<IActionResult> Index()
        {
            var toCheck = unitOfWork;
            //var isDoctor = User
            var isDoctor = false;
            foreach (var p in unitOfWork.Psychologists.GetAll())
            {
                if (p.Name + p.Surname == User.Identity.Name)
                {
                    isDoctor = true;
                }
            }
            //var isDoctor = false;
            int customerId;
            try
            {
                var hz = User.Identity.Name;
                //var hz = toCheck;
                customerId = isDoctor
                ? unitOfWork.Psychologists.GetAll().Where(c => c.Name + c.Surname == User.Identity.Name).First().PsychologistId
                : unitOfWork.Customers.GetAll().Where(c => c.Name + c.Surname == User.Identity.Name).First().CustomerId;
            }
            catch
            {
                var hz = User.Identity.Name;
                //var hz = toCheck;
                customerId = isDoctor
                ? unitOfWork.Psychologists.GetAll().Where(c => c.Email == User.Identity.Name).First().PsychologistId
                : unitOfWork.Customers.GetAll().Where(c => c.Email == User.Identity.Name).First().CustomerId;
            }
            Customer customer = await unitOfWork.Customers.Get(customerId);
            var customerRecords = new List<Record>();

            customerRecords.AddRange(unitOfWork.Records.GetAll().Where(o => o.CustomerId == customer.CustomerId));

            return View(customerRecords);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateRecordView
            {
                Psychologists = unitOfWork.Psychologists.GetAll().ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRecordView model)
        {
            var psychologist = Request.Form["psychologistSelect"];
            if (psychologist == StringValues.Empty)
            {
                ViewBag.Message = "No psychologist for now please try later or write us on PsychoSupport@gmail.com";

                return View("ErrorPage");
            }
            var record = new Record
            {
                CustomerId = unitOfWork.Customers.GetAll().First(c => c.Email == User.Identity.Name)
                    .CustomerId,
                Psychologist =
                    unitOfWork.Psychologists.GetAll().First(p => p.Surname == psychologist),
                InitialDate = DateTime.Now,
                AssignedDate = model.AssignedDate,
                ATime = model.ATime
            };


            await unitOfWork.Records.Create(record);
            await unitOfWork.SaveAsync();

            return RedirectToAction("Index", "User");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            Record record = await unitOfWork.Records.Get(id);

            if (record != null)
            {
                await unitOfWork.Records.Delete(id);
                await unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
