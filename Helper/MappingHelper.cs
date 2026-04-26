using BlogFlow.API.DTOs.Categories;
using BlogFlow.API.DTOs.Post;
using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Models;

namespace BlogFlow.API.Helper
{
    public static class MappingHelper
    {
        public static PostReadDTO PostToDTO(Post post)
        {
            return new PostReadDTO
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,

                AuthorId = post.AuthorId,
                AuthorUsername = post.Author.Username,

                CategoryId = post.CategoryId,
                CategoryName = post.Category?.Name,

                Tags = post.PostTags
                     .Select(pt => new TagReadDTO
                     {
                         Id = pt.Tag.Id,
                         Name = pt.Tag.Name
                     })
                     .ToList()
            };
        }

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
