using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PsychoWeb.Models.Repositories
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();

        Task<T> Get(int id);

        Task Create(T item);

        void Update(T item);

        Task Delete(int id);
    }
}
