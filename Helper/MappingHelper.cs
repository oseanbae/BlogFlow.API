using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.DTOs.Post;
using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Models;

namespace BlogFlow.API.Helper
{
    public static class MappingHelper
    {
        public static CategoryReadDTO CategoryToDTO(Category category)
        {
            return new CategoryReadDTO
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}
