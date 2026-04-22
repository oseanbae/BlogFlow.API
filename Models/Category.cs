namespace BlogFlow.API.Models
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public ICollection<Post> Posts { get; private set; } = [];
    }
}
