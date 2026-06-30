using carwash.Data;
using WashServiceEntity = carwash.Data.Entities.WashService;
using carwash.Service.DTOs.Common;
using carwash.Service.DTOs.WashServices;
using carwash.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace carwash.Service.Services;

public class WashServicesService : IWashServicesService
{
    private readonly ApplicationDbContext _dbContext;

    public WashServicesService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceResult<IReadOnlyList<WashServiceDto>>> GetAllAsync()
    {
        var services = await _dbContext.WashServices
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Select(s => new WashServiceDto
            {
                Id = s.Id,
                NameAr = s.NameAr,
                NameEn = s.NameEn,
                Points = s.Points
            })
            .ToListAsync();

        return ServiceResult<IReadOnlyList<WashServiceDto>>.Ok(services);
    }

    public async Task<ServiceResult<WashServiceDto>> GetByIdAsync(int id)
    {
        var service = await FindServiceAsync(id);
        if (service is null)
        {
            return ServiceResult<WashServiceDto>.Fail("Service not found.");
        }

        return ServiceResult<WashServiceDto>.Ok(MapToDto(service));
    }

    public async Task<ServiceResult<WashServiceDto>> CreateAsync(CreateWashServiceRequest request)
    {
        var nameAr = request.NameAr.Trim();
        var nameEn = request.NameEn.Trim();

        if (await NameArExistsAsync(nameAr))
        {
            return ServiceResult<WashServiceDto>.Fail("A service with this Arabic name already exists.");
        }

        if (await NameEnExistsAsync(nameEn))
        {
            return ServiceResult<WashServiceDto>.Fail("A service with this English name already exists.");
        }

        var service = new WashServiceEntity
        {
            NameAr = nameAr,
            NameEn = nameEn,
            Points = request.Points
        };

        _dbContext.WashServices.Add(service);
        await _dbContext.SaveChangesAsync();

        return ServiceResult<WashServiceDto>.Ok(MapToDto(service));
    }

    public async Task<ServiceResult<WashServiceDto>> UpdateAsync(int id, UpdateWashServiceRequest request)
    {
        var service = await FindServiceAsync(id);
        if (service is null)
        {
            return ServiceResult<WashServiceDto>.Fail("Service not found.");
        }

        var nameAr = request.NameAr.Trim();
        var nameEn = request.NameEn.Trim();

        if (!string.Equals(service.NameAr, nameAr, StringComparison.OrdinalIgnoreCase)
            && await NameArExistsAsync(nameAr))
        {
            return ServiceResult<WashServiceDto>.Fail("A service with this Arabic name already exists.");
        }

        if (!string.Equals(service.NameEn, nameEn, StringComparison.OrdinalIgnoreCase)
            && await NameEnExistsAsync(nameEn))
        {
            return ServiceResult<WashServiceDto>.Fail("A service with this English name already exists.");
        }

        service.NameAr = nameAr;
        service.NameEn = nameEn;
        service.Points = request.Points;

        await _dbContext.SaveChangesAsync();

        return ServiceResult<WashServiceDto>.Ok(MapToDto(service));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var service = await FindServiceAsync(id);
        if (service is null)
        {
            return ServiceResult<bool>.Fail("Service not found.");
        }

        _dbContext.WashServices.Remove(service);
        await _dbContext.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    private async Task<WashServiceEntity?> FindServiceAsync(int id) =>
        await _dbContext.WashServices.FirstOrDefaultAsync(s => s.Id == id);

    private async Task<bool> NameArExistsAsync(string nameAr) =>
        await _dbContext.WashServices.AnyAsync(s => s.NameAr == nameAr);

    private async Task<bool> NameEnExistsAsync(string nameEn) =>
        await _dbContext.WashServices.AnyAsync(s => s.NameEn == nameEn);

    private static WashServiceDto MapToDto(WashServiceEntity service) => new()
    {
        Id = service.Id,
        NameAr = service.NameAr,
        NameEn = service.NameEn,
        Points = service.Points
    };
}
