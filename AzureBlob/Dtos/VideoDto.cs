namespace AzureBlob1.Dtos;

public class VideoDto
{
    public string Title { get; set; } = string.Empty;
    public IFormFile Video { get; set; } = default!;
    public IFormFile VideoImage { get; set; } = default!;
 
}
