using Core.Supports.Bases;
using Database.Models;
using Database.Repositories;
using Database.Supports.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Core.BizLogics
{
    public class UsersBizLogic : BaseBizLogic<UsersRepository, UserModel>
    {
        public UsersBizLogic(IConfiguration configuration, PopAppContext context) : base(configuration)
        {
            Repository = new UsersRepository(context);
        }

        public async Task<UserModel> GetOrInsertUser(UserModel userCredentials, IDbContextTransaction inheritedTransaction = null)
        {
            var dbContextTransaction = inheritedTransaction ?? Repository.Context.Database.BeginTransaction();

            try
            {
                if (string.IsNullOrWhiteSpace(userCredentials.UserName) || string.IsNullOrWhiteSpace(userCredentials.Password))
                {
                    if (inheritedTransaction == null)
                    {
                        dbContextTransaction.Commit();
                    }

                    return null;
                }

                var user = await Repository.GetUserByUserName(userCredentials.UserName);

                if (user == null)
                {
                    user = await Repository.Insert(new UserModel()
                    {
                        UserName = userCredentials.UserName,
                        Password = new PasswordHasher<string>().HashPassword(userCredentials.UserName,userCredentials.Password)
                    });
                }

                if (inheritedTransaction == null)
                {
                    dbContextTransaction.Commit();
                }

                return user;
            }
            catch (Exception)
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
