using BlogFlow.API.Domain.Entities;

namespace BlogFlow.API.Domain.QueryParams;

public class PostQueryParams
{
    private int _page = 1;
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 1 : (value > 100 ? 100 : value);
    }

    public PostState? State { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? TagId { get; set; }
    public Guid? AuthorId { get; set; }
    public string? Keyword { get; set; }
}
