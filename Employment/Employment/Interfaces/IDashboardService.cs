using System.Threading.Tasks;
using Employment.ViewModels;

namespace Employment.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
    }
}