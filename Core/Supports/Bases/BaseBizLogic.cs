using Core.Support.Interfaces;
using Database.Supports.Bases;
using Database.Supports.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace Core.Supports.Bases
{
    public abstract class BaseBizLogic<Y, T> : IBizLogic<T>
        where T : BaseModel
        where Y : IRepository<T>
    {
        protected Y Repository;
        protected readonly IConfiguration Configuration;

        public BaseBizLogic(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected object QueryStringToObject(IDictionary<string, string> source)
        {
            object propertiesObject = null;

            if (source != null && source.Count > 0)
            {
                var eo = new ExpandoObject();
                var eoColl = (ICollection<KeyValuePair<string, object>>)eo;

                foreach (var item in source)
                {
                    var property = item.Key[0].ToString().ToUpper() + item.Key.Substring(1, item.Key.Length - 1);
                    eoColl.Add(new KeyValuePair<string, object>(property, item.Value));
                }

                dynamic eoDynamic = eo;
                propertiesObject = eoDynamic;
            }

            return propertiesObject;
        }

        public virtual async Task<T> Get(int id, IDbContextTransaction inheritedTransaction = null)
        {
            var dbContextTransaction = inheritedTransaction ?? Repository.Context.Database.BeginTransaction();

            try
            {

                var result = await Repository.Get(id);

                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Commit();
                }

                return result;
            }
            catch
            {
                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Rollback();
                }
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAll(IDictionary<string, string> query = null, IDbContextTransaction inheritedTransaction = null)
        {
            var dbContextTransaction = inheritedTransaction ?? Repository.Context.Database.BeginTransaction();

            try
            {
                var parametersObject = QueryStringToObject(query);

                var result = await Repository.GetAll(parametersObject);

                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Commit();
                }

                return result;
            }
            catch
            {
                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Rollback();
                }

                throw;
            }
        }

        public virtual async Task<T> Insert(T entity, IDbContextTransaction inheritedTransaction = null)
        {
            var dbContextTransaction = inheritedTransaction ?? Repository.Context.Database.BeginTransaction();

            try
            {
                var result = await Repository.Insert(entity);

                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Commit();
                }

                return result;
            }
            catch
            {
                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Rollback();
                }
                throw;
            }
        }

        public virtual async Task<T> Update(int id, T entity, IDbContextTransaction inheritedTransaction = null)
        {
            var dbContextTransaction = inheritedTransaction ?? Repository.Context.Database.BeginTransaction();

            try
            {
                var result = await Repository.Update(id, entity);

                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Commit();
                }

                return result;
            }
            catch
            {
                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Rollback();
                }
                throw;
            }
        }

        public virtual async Task Delete(int id, IDbContextTransaction inheritedTransaction = null)
        {
            var dbContextTransaction = inheritedTransaction ?? Repository.Context.Database.BeginTransaction();

            try
            {
                await Repository.Delete(id);

                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Commit();
                }
            }
            catch
            {
                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Rollback();
                }
                throw;
            }
        }
    }
}
