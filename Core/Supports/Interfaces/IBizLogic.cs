using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Support.Interfaces
{
    public interface IBizLogic<T> 
        where T: class
    {
        Task<IEnumerable<T>> GetAll(IDictionary<string, string> query = null, IDbContextTransaction inheritedTransaction = null);
        Task<T> Get(int id, IDbContextTransaction inheritedTransaction = null);
        Task<T> Insert(T entity, IDbContextTransaction inheritedTransaction = null);
        Task<T> Update(int id, T entity, IDbContextTransaction inheritedTransaction = null);
        Task Delete(int id, IDbContextTransaction inheritedTransaction = null);
    }
}
