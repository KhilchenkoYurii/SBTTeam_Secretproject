using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PsychoWeb.Context;

namespace PsychoWeb.Models.Repositories
{
    public class CustomerRepository : IRepository<Customer>
    {
        private readonly AppDbContext applicationContext;

        public CustomerRepository(AppDbContext appDbContext)
        {
            this.applicationContext = appDbContext;
        }

        public async Task Create(Customer item)
        {
            await this.applicationContext.Customers.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            Customer customer = await applicationContext.Customers.FindAsync(id);

            if (customer != null)
            {
                applicationContext.Customers.Remove(customer);
            }
        }

        public async Task<Customer> Get(int id)
        {
            Customer customer = await applicationContext.Customers.FindAsync(id);

            return customer;
        }

        public IEnumerable<Customer> GetAll()
        {
            IEnumerable<Customer> customers = applicationContext.Customers;

            return customers;
        }

        public void Update(Customer item)
        {
            applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}
