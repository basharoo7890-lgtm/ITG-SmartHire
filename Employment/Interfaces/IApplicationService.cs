namespace Employment.Interfaces
{
    public interface IApplicationService
    {
        Task ProcessAutoFilterAsync(int applicationId);
    }
}