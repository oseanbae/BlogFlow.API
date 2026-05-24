namespace BlogFlow.API.DTOs.Admin
{
    public class AdminStatsDTO
    {
        public int TotalUsers { get; init; }

        public int ActiveUsers { get; init; }

        public int DeletedUsers { get; init; }

        public int AdminCount { get; init; }

        public int AuthorCount { get; init; }

        public int ReaderCount { get; init; }

        public int NewUsersThisWeek { get; init; }

        public int NewUsersThisMonth { get; init; }
    }
}
