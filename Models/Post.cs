using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogFlow.API.Models
{
    public class Post
    {
        //[PK]
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(255)]
        public required string Title { get; set; }

        [Required, MaxLength(10000)]
        public required string Body { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        //[FK]
        public Guid AuthorId { get; set; }
        public Guid CategoryId { get; set; }

        //Navigation Properties
        public User Author { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<PostTag> PostTags { get; set; } = [];


    }
}
