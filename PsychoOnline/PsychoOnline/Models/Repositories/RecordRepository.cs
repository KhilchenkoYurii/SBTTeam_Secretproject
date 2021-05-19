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
    public class RecordRepository:IRepository<Record>
    {
        private readonly AppDbContext applicationContext;

        public RecordRepository(AppDbContext appDbContext)
        {
            this.applicationContext = appDbContext;
        }

        public async Task Create(Record item)
        {
            await this.applicationContext.Records.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            Record record = await applicationContext.Records.FindAsync(id);

            if (record != null)
            {
                applicationContext.Records.Remove(record);
            }
        }

        public async Task<Record> Get(int id)
        {
            Record record = await applicationContext.Records.FindAsync(id);

            return record;
        }

        public IEnumerable<Record> GetAll()
        {
            IEnumerable<Record> records = applicationContext.Records.Include(p=>p.Psychologist);

            return records;
        }

        public void Update(Record item)
        {
            applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}
