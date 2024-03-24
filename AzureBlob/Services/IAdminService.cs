using AzureBlob1.Dtos;
using AzureBlob1.Entities;

namespace AzureBlob1.Services;

public interface IAdminService
{
    public Task<Admin> CreateAsync(AdminCreateDto dto);
    public Task<(bool Result, string Token)> LoginAsync(LoginDto dto);
}
