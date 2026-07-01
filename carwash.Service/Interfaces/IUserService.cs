using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.Users;

namespace carwash.Service.Interfaces;

public interface IUserService
{
    Task<ServiceResult<IReadOnlyList<CustomerDto>>> GetCustomersAsync(
        bool activeOnly = false,
        int? year = null,
        int? month = null,
        int? day = null);

    Task<ServiceResult<bool>> DeleteCustomerAsync(string userId);
}
