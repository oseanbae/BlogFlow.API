namespace BlogFlow.API.Models
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; private set; } = null!;
        public ICollection<Post> Posts { get; private set; } = [];

        //Constructor for EF Core to load data from db
        private Category() { }

        public Category(string name)
        {
            Name = Normalize(name);
        }
        public void Rename(string newName)
        {
            Name = Normalize(newName);
        }
        public static string Normalize(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");

            return name.Trim().ToLowerInvariant();
        }
    }
}