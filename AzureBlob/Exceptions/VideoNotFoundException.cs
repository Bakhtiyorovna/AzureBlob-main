namespace AzureBlob1.Exceptions;

public class VideoNotFoundException : NotFoundException
{
    public VideoNotFoundException()
    {
        this.TitleMessage = "Video/Videos not found";
    }
}
