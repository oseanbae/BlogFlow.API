using BlogFlow.API.Exceptions;

namespace BlogFlow.API.Domain.Entities;

// Join entity for many-to-many relationship between Post and Tag
public class PostTag
{
    public Guid PostId { get; private set; }
    public Guid TagId { get; private set; }

    public Post Post { get; private set; } = null!;
    public Tag Tag { get; private set; } = null!;
    private PostTag() { } // EF Core

    public PostTag(Guid postId, Guid tagId)
    {
        if (postId == Guid.Empty)
            throw new BadRequestException("Invalid PostId", "INVALID_POST_ID");

        if (tagId == Guid.Empty)
            throw new BadRequestException("Invalid TagId", "INVALID_TAG_ID");

        PostId = postId;
        TagId = tagId;
    }
}
