using BlogFlow.API.DTOs.Post;
using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Models;

namespace BlogFlow.API.Queries
{
    public static class PostQueries
    {
        public static IQueryable<PostReadDTO> AsDTO(this IQueryable<Post> query)
        {
            return query.Select(p => new PostReadDTO
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,

                AuthorId = p.AuthorId,
                AuthorUsername = p.Author.Username,

                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,

                Tags = p.PostTags
                    .Select(pt => new TagReadDTO
                    {
                        Id = pt.Tag.Id,
                        Name = pt.Tag.Name
                    })
                    .ToList()
            });
        }
    }
}