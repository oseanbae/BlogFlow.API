using BlogFlow.API.DTOs.Post;
using BlogFlow.API.DTOs.Tag;
using BlogFlow.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogFlow.API.QueryExtensions
{
    public static class PostQueryExtensions
    {
        // For Database Queries (translates to SQL)
        public static IQueryable<PostReadDTO> AsDTO(this IQueryable<Post> query)
        {
            return query
                .AsNoTracking()
                .Select(p => new PostReadDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Body = p.Body,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,

                    AuthorId = p.AuthorId,
                    AuthorUsername = p.Author.Username,

                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                    Tags = p.PostTags.Select(pt => new TagReadDTO
                    {
                        Id = pt.Tag.Id,
                        Name = pt.Tag.Name,
                        DisplayName = pt.Tag.DisplayName
                    })
                });
        }
        public static IEnumerable<PostReadDTO> AsDTO(this IEnumerable<Post> posts)
        {
            return posts.Select(p => p.ToDTO());
        }

        // Single Object Mapper
        public static PostReadDTO ToDTO(this Post post)
        {
            return new PostReadDTO
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                AuthorId = post.AuthorId,
                AuthorUsername = post.Author?.Username ?? string.Empty,
                CategoryId = post.CategoryId,
                CategoryName = post.Category?.Name ?? string.Empty,
                Tags = post.PostTags?
                    .Select(pt => new TagReadDTO
                    {
                        Id = pt.Tag.Id,
                        Name = pt.Tag.Name,
                        DisplayName = pt.Tag.DisplayName 
                    }).ToList() ?? []
            };
        }
    }
}