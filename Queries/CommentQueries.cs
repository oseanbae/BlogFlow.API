using BlogFlow.API.DTOs.Comment;
using BlogFlow.API.Models;
using System.Linq.Expressions;

namespace BlogFlow.API.Queries
{
    public static class CommentQueries
    {
       
        public static readonly Expression<Func<Comment, CommentReadDTO>> Map = comment => new CommentReadDTO
        {
            Id = comment.Id,
            PostId = comment.PostId,
            UserId = comment.UserId,
            AuthorName = comment.User!.Username, // EF translates this to a JOIN in SQL
            Body = comment.Body,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };

        // Compiled version for ToDTO (In-memory)
        private static readonly Func<Comment, CommentReadDTO> _mapFunc = Map.Compile();

        public static IQueryable<CommentReadDTO> AsDTO(this IQueryable<Comment> query)
        {
            return query.Select(Map); // Uses the Expression (SQL)
        }

        public static CommentReadDTO ToDTO(this Comment comment)
        {
            return _mapFunc(comment); // Uses the compiled function (In-Memory)
        }

    }
}
