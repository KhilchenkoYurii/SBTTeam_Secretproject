using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PsychoOnline.Models.Repositories;
using PsychoWeb.Context;
using PsychoWeb.Models.Repositories;

namespace PsychoWeb.Models
{
    public class UnitOfWork : IDisposable
    {
        private readonly AppDbContext applicationContext;

        private CustomerRepository customerRepository;
        private RecordRepository recordRepository;
        private PsychologistRepository psychologistRepository;

        private bool disposed = false;

        public UnitOfWork(AppDbContext appDbContext)
        {
            this.applicationContext = appDbContext;
        }

        public CustomerRepository Customers
        {
            get
            {
                if (customerRepository == null)
                {
                    customerRepository = new CustomerRepository(this.applicationContext);
                }

                return customerRepository;
            }
        }

        public PsychologistRepository Psychologists
        {
            get
            {
                if (psychologistRepository == null)
                {
                    psychologistRepository = new PsychologistRepository(this.applicationContext);
                }

                return psychologistRepository;
            }
        }

        public RecordRepository Records
        {
            get
            {
                if (recordRepository == null)
                {
                    recordRepository = new RecordRepository(this.applicationContext);
                }

                return recordRepository;
            }
        }

        public async Task SaveAsync()
        {
            await applicationContext.SaveChangesAsync();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    applicationContext.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public AppDbContext GetContext()
        {
            return this.applicationContext;
        }
    }
}
