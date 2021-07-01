using System.ComponentModel.DataAnnotations;

namespace Database.Supports.Bases
{
    public abstract class BaseModel
    {
        [Key]
        public int Id { get; set; }
    }
}
