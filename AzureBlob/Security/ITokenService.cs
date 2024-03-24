using AzureBlob1.Entities;

namespace AzureBlob1.Security;

public interface ITokenService
{
    public string GenerateToken(Admin admin);
}
