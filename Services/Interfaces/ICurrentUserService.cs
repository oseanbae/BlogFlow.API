namespace BlogFlow.API.Services.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? GetSubClaimUserId();
        Guid GetRequiredUserId();
    }
}
