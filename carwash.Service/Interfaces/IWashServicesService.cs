using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.WashServices;

namespace carwash.Service.Interfaces;

public interface IWashServicesService
{
    Task<ServiceResult<IReadOnlyList<WashServiceDto>>> GetAllAsync();
    Task<ServiceResult<WashServiceDto>> GetByIdAsync(int id);
    Task<ServiceResult<WashServiceDto>> CreateAsync(CreateWashServiceRequest request);
    Task<ServiceResult<WashServiceDto>> UpdateAsync(int id, UpdateWashServiceRequest request);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
