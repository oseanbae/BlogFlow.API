namespace BlogFlow.API.DTOs.Admin
{
    public class AdminStatsDTO
    {
        public int TotalUsers { get; set; }

        public int ActiveUsers { get; set; }

        public int DeletedUsers { get; set; }

        public int AdminCount { get; set; }

        public int AuthorCount { get; set; }

        public int ReaderCount { get; set; }

        public int NewUsersThisWeek { get; set; }

        public int NewUsersThisMonth { get; set; }
    }
}
