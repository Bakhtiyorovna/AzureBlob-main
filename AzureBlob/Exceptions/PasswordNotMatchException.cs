namespace AzureBlob1.Exceptions;

public class PasswordNotMatchException : BadRequestException
{
    public PasswordNotMatchException()
    {
        this.TitleMessage = "Invalid Password";
    }
}
