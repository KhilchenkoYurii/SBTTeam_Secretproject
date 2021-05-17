using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PsychoWeb.Context;

namespace PsychoWeb.Models
{
    public class DataBaseInitializer
    {

        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            AppDbContext appContext)
        {
            AppDbContext appDbContext = appContext;
            UnitOfWork unitOfWork = new UnitOfWork(appDbContext);
            string adminEmail = "administrator123@gmail.com";
            string adminPassword = "Admin123";

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            if (await roleManager.FindByNameAsync("customer") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("customer"));
            }

            if (await roleManager.FindByNameAsync("manager") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("manager"));
            }

            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                ApplicationUser admin = new ApplicationUser { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }

            if (appDbContext != null)
            {
                if (unitOfWork.Customers.GetAll().Count() == 0)
                {
                    await unitOfWork.Customers.Create(
                        new Customer()
                        {
                            CustomerId = 1,
                            Name = "Adam",
                            Surname = "Rowan",
                            Email = "asn@gmail.com"
                        });
                    await unitOfWork.SaveAsync();
                }
                

                //if (unitOfWork.Orders.GetAll().Count() == 0)
                //{
                //    await unitOfWork.Orders.Create(new Order()
                //    {
                //        OrderDate = new DateTime(2017, 7, 12),
                //        Customer = await unitOfWork.Customers.Get(appContext.Customers.First().Id),
                //        OrderStatus = OrderStatus.Cancelled
                //    });

                //    await unitOfWork.SaveAsync();
                //}
            }
        }
    }
}
