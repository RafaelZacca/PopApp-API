using Database.Supports.Bases;
using Database.Supports.Contexts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database.Supports.Interfaces
{
    public interface IRepository<T> where T : BaseModel
    {
        PopAppContext Context { get; }

        Task<IEnumerable<T>> GetAll(object parametersObject = null);
        Task<T> Get(int id);
        Task<T> Insert(T entity);
        Task<T> Update(int id, T entity);
        Task Delete(int id);
    }
}
