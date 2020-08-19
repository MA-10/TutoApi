using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Models;

namespace Test.Services
{
    public interface IApplicationRepository <T>
    {
        IQueryable<T> Get();

        T Get(Guid id);

        Task<T> GetAsync(Guid id);

        void Create(T record);

        void Update(T record);

        void Delete(Guid id);

        int Save();

        Task<int> SaveAsync();

        Task<List<T>> GetAllPageableAsync(PaginationFilter pagination, SearchParameter[] parameters);
    }
}