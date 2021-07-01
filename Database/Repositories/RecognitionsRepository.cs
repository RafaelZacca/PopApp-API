using Database.Models;
using Database.Supports.Bases;
using Database.Supports.Contexts;

namespace Database.Repositories
{
    public class RecognitionsRepository: BaseRepository<RecognitionModel>
    {
        public RecognitionsRepository(PopAppContext context) : base(context, context.Recognitions)
        {

        }
    }
}
