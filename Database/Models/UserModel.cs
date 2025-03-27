using Database.Supports.Bases;

namespace Database.Models
{
    public class UserModel: BaseModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
