using BlogFlow.API.Exceptions;

namespace BlogFlow.API.Domain.Entities;

public enum PostState
{
    Draft = 1,
    Published = 2,
    Archived = 3
}

public class Post
{
    //PK
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public PostState State { get; private set; } = PostState.Draft;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    //[FK]
    public Guid AuthorId { get; private set; }
    public Guid CategoryId { get; private set; }

    //Navigation Properties
    public User Author { get; private set; } = null!;
    public Category Category { get; private set; } = null!;
    public ICollection<PostTag> PostTags { get; private set; } = [];
    public ICollection<Comment> Comments { get; private set; } = [];

    //Constructor for EF Core to load data from db
    private Post() { }

    public Post(string title, string body, Guid authorId, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new BadRequestException("Title is required", "EMPTY_POST_TITLE");

        if (string.IsNullOrWhiteSpace(body))
            throw new BadRequestException("Body is required", "EMPTY_POST_BODY");

        if (authorId == Guid.Empty)
            throw new BadRequestException("Invalid author", "INVALID_AUTHOR_ID");

        if (categoryId == Guid.Empty)
            throw new BadRequestException("Invalid category", "INVALID_CATEGORY_ID");

        Title = title.Trim();
        Body = body;
        State = PostState.Draft;
        AuthorId = authorId;
        CategoryId = categoryId;
    }

    // SEED constructor
    public Post(Guid id, string title, string body, Guid authorId, Guid categoryId)
        : this(title, body, authorId, categoryId)
    {
        Id = id;
    }

    public void Publish()
    {
        if (DeletedAt.HasValue)
            throw new ConflictException("Cannot publish a deleted post. It must be restored first.", "POST_IS_DELETED");

        if (State == PostState.Published)
            throw new ConflictException("This post is already live.", "POST_ALREADY_PUBLISHED");

        State = PostState.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        if (DeletedAt.HasValue)
            throw new ConflictException("Cannot archive a deleted post. It must be restored first.", "POST_IS_DELETED");

        if (State == PostState.Archived)
            throw new ConflictException("This post is already archived.", "POST_ALREADY_ARCHIVED");

        if (State != PostState.Published) 
            throw new ConflictException("Only published posts can be archived.", "POST_NOT_PUBLISHED");

        State = PostState.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    // Use case 1: Taking a live post down to edit it
    public void Unpublish()
    {
        if (DeletedAt.HasValue)
            throw new ConflictException("Cannot unpublish a deleted post.", "POST_IS_DELETED");

        if (State != PostState.Published)
            throw new ConflictException("Only published posts can be unpublished.", "POST_NOT_PUBLISHED");

        State = PostState.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    // Use case 2: Bringing an old post out of retirement
    public void MoveToDraft()
    {
        if (DeletedAt.HasValue)
            throw new ConflictException("Cannot move a deleted post to draft. It must be restored first.", "POST_IS_DELETED");

        if (State == PostState.Draft)
            throw new ConflictException("This post is already a draft.", "POST_ALREADY_DRAFT");

        if (State != PostState.Archived)
            throw new ConflictException("Only archived posts can be restored to draft.", "POST_NOT_ARCHIVED");

        State = PostState.Draft;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (DeletedAt != null) throw new ConflictException($"Post '{Id}' is already deleted.", "POST_ALREADY_DELETED");
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (DeletedAt == null) throw new ConflictException($"Post '{Id}' is not deleted.", "POST_NOT_DELETED");
        DeletedAt = null;
        State = PostState.Draft;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Update(string title, string body, Guid categoryId)
    {
        if (DeletedAt != null)
            throw new BadRequestException("Cannot update a deleted post.", "POST_ALREADY_DELETED");

        if (string.IsNullOrWhiteSpace(title))
            throw new BadRequestException("Title is required", "EMPTY_POST_TITLE");

        if (string.IsNullOrWhiteSpace(body))
            throw new BadRequestException("Body is required", "EMPTY_POST_BODY");

        if (categoryId == Guid.Empty)
            throw new BadRequestException("Invalid category", "INVALID_CATEGORY_ID");

        Title = title.Trim();
        Body = body;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }
    public void SetTags(IEnumerable<Guid> tagIds)
    {
        if (tagIds == null)
            return;

        var newTagIds = tagIds.Distinct().ToHashSet();
        var existingTagIds = PostTags.Select(pt => pt.TagId).ToHashSet();

        var toRemove = PostTags
            .Where(pt => !newTagIds.Contains(pt.TagId))
            .ToList();

        foreach (var tag in toRemove)
        {
            PostTags.Remove(tag);
        }

        var toAdd = newTagIds
            .Where(id => !existingTagIds.Contains(id));

        foreach (var tagId in toAdd)
        {
            PostTags.Add(new PostTag(Id, tagId));
        }
    }
}
