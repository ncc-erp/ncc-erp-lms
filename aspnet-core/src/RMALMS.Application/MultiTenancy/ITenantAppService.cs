using Abp.Application.Services;
using Abp.Application.Services.Dto;
using RMALMS.MultiTenancy.Dto;

namespace RMALMS.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
