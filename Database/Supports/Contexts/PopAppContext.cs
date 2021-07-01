using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Supports.Contexts
{
    public class PopAppContext: DbContext
    {
        public PopAppContext(DbContextOptions<PopAppContext> options) : base(options)
        {
        }

        public DbSet<RecognitionModel> Recognitions { get; set; }
        public DbSet<UserModel> Users { get; set; }
    }
}
