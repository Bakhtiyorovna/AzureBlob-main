namespace AzureBlob1.Exceptions;

public class AdminNotFoundException : NotFoundException
{
    public AdminNotFoundException()
    {
        this.TitleMessage = "Admin not Found";
    }
}
