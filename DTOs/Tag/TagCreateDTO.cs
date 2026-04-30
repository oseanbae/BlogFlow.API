namespace BlogFlow.API.DTOs.Tag
{
    public class TagCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public TagCreateDTO(string name) 
        {
            Name = name;
        }
    }
}
