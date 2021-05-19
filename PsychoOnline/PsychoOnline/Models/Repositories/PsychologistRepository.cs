using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PsychoOnline.Context;
using PsychoWeb.Context;
using PsychoWeb.Models.Repositories;

namespace PsychoOnline.Models.Repositories
{
    public class PsychologistRepository:IRepository<Psychologist>
    {
        private readonly AppDbContext applicationContext;

        public PsychologistRepository(AppDbContext appDbContext)
        {
            this.applicationContext = appDbContext;
        }

        public async Task Create(Psychologist item)
        {
            await this.applicationContext.Psychologists.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            Psychologist psychologist = await applicationContext.Psychologists.FindAsync(id);

            if (psychologist != null)
            {
                applicationContext.Psychologists.Remove(psychologist);
            }
        }

        public async Task<Psychologist> Get(int id)
        {
            Psychologist psychologist = await applicationContext.Psychologists.FindAsync(id);
            
            return psychologist;
        }

        public IEnumerable<Psychologist> GetAll()
        {
            IEnumerable<Psychologist> psychologists = applicationContext.Psychologists;

            return psychologists;
        }

        public void Update(Psychologist item)
        {
            applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}
