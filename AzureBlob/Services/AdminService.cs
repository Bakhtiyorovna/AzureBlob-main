using AzureBlob1.Dtos;
using AzureBlob1.Entities;
using AzureBlob1.Exceptions;
using AzureBlob1.Repositories;
using AzureBlob1.Security;

namespace AzureBlob1.Services;

public class AdminService : IAdminService
{
    private readonly IRepository<Admin> _repository;
    private readonly ITokenService _tokenService;

    public AdminService(IRepository<Admin> repository,
                        ITokenService tokenService)
    {
        _repository = repository;
        _tokenService = tokenService;
    }

    public async Task<Admin> CreateAsync(AdminCreateDto dto)
    {   
        Admin admin = new Admin();
        admin.Name = dto.Name;
        admin.Email = dto.Email;

        var hasherResult = PasswordHasher.Hash(dto.Password);
        admin.PasswordHash = hasherResult.Hash;
        admin.Salt = hasherResult.Salt;
        await _repository.AddAsync(admin);
        await _repository.SaveAsync();

        return admin;
    }

    public async Task<(bool Result, string Token)> LoginAsync(LoginDto dto)
    {
        var admin = await _repository.SelectAsync(x => x.Email == dto.Email);
        if (admin == null) throw new AdminNotFoundException();

        var haserResult = PasswordHasher.Verify(dto.Password, admin.PasswordHash, admin.Salt);
        if (haserResult == false) throw new PasswordNotMatchException();

        string token = _tokenService.GenerateToken(admin);

        return(Result: true, Token: token);
    }
}
