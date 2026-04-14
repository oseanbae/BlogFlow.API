using System.ComponentModel.DataAnnotations;

namespace BlogFlow.API.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public required string Name { get; set; }
        public ICollection<Post> Posts { get; set; } = [];
    }
}
