using System.Threading.Tasks;
using Abp.Application.Services;
using RMALMS.Sessions.Dto;

namespace RMALMS.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
