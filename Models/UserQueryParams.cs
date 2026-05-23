namespace BlogFlow.API.Models
{ 
    public class UserQueryParams
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

        public string? Search { get; set; }
        public UserRole? Role { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedAfter { get; set; }
    }
}
