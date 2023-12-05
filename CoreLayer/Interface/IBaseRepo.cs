using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Interface
{
    public interface IBaseRepo<T> where T : class
    {
        Task<T> GetByidAsync(string id);
        Task<T> GetByidAsync(int id);
        Task<T> GetByidAsync(int? id);

        Task<bool> CreateAsync(T entity);
        IQueryable<T> GetAll();
        Task<T> Update(T entity);
        Task<bool> Delete(string id);
        Task<bool> Delete(int id);


    }
}
