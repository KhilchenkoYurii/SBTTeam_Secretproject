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
    public class ManagerController:Controller
    {
        private readonly UnitOfWork unitOfWork;

        public ManagerController(AppDbContext appDbContext)
        {
            this.unitOfWork = new UnitOfWork(appDbContext);
        }

        public async Task<IActionResult> Index()
        {
            int managerId = unitOfWork.Psychologists.GetAll().Where(c => c.Email == User.Identity.Name).First().PsychologistId;
            Psychologist psychologist = await unitOfWork.Psychologists.Get(managerId);
            var managerRecords = new List<Record>();
            
            managerRecords.AddRange(unitOfWork.Records.GetAll().Where(o => o.Psychologist.PsychologistId == psychologist.PsychologistId));
            foreach (var i in managerRecords)
            {
                i.Customer = await unitOfWork.Customers.Get(i.CustomerId);
            }

            await unitOfWork.SaveAsync();
            return View(managerRecords);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreatePsychologistRecordView
            {
                Customers = unitOfWork.Customers.GetAll().ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePsychologistRecordView model)
        {
            var customer = Request.Form["psychologistSelect"];
            if (customer == StringValues.Empty)
            {
                ViewBag.Message = "No customers for now please try later";

                return View("ErrorPage");
            }

            var record = new Record
            {
                CustomerId = unitOfWork.Customers.GetAll().First(p => p.Surname == customer).CustomerId,
                Psychologist = unitOfWork.Psychologists.GetAll().First(c => c.Email == User.Identity.Name),
                InitialDate = DateTime.Now,
                AssignedDate = model.AssignedDate,
                ATime = model.ATime
            };


            await unitOfWork.Records.Create(record);
            await unitOfWork.SaveAsync();

            return RedirectToAction("Index", "Manager");
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

            return RedirectToAction("Index", "Manager");
        }
    }
}
