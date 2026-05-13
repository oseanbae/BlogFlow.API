using BlogFlow.API.DTOs.Comment;
using BlogFlow.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlogFlow.API.QueryExtensions
{
    public static class CommentQueryExtensions
    {
       
        public static readonly Expression<Func<Comment, CommentReadDTO>> Map = comment => new CommentReadDTO
        {
            Id = comment.Id,
            PostId = comment.PostId,
            UserId = comment.UserId,
            AuthorName = comment.User.Username, // EF translates this to a JOIN in SQL
            Body = comment.Body,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };

        // Compiled version for ToDTO (In-memory)
        private static readonly Func<Comment, CommentReadDTO> _mapFunc = Map.Compile();

        // For IQueryable (SQL)
        public static IQueryable<CommentReadDTO> AsDTO(this IQueryable<Comment> query)
        {
            return query.AsNoTracking().Select(Map);
        }

        // For IEnumerable (Memory) 
        public static IEnumerable<CommentReadDTO> AsDTO(this IEnumerable<Comment> comments)
        {
            return comments.Select(ToDTO);
        }

        public static CommentReadDTO ToDTO(this Comment comment)
        {
            return _mapFunc(comment);
        }

    }
}
