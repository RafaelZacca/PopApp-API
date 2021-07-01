using Database.Models;
using Database.Supports.Bases;
using Database.Supports.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Repositories
{
    public class UsersRepository : BaseRepository<UserModel>
    {
        public UsersRepository(PopAppContext context) : base(context, context.Users)
        {

        }

        public async Task<UserModel> GetUserByUserName(string userName)
        {
            IQueryable<UserModel> query = DbSet;
            return await query?.FirstOrDefaultAsync(x => x.UserName == userName);
        }
    }
}
