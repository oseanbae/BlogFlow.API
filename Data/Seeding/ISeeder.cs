namespace BlogFlow.API.Data.Seeding
{
    public interface ISeeder
    {
        int Order { get; } //Orer of Execution
        Task SeedAsync(AppDbContext db);
    }
}
