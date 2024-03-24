namespace AzureBlob1.Dtos;

public class BlobDto
{
    public string Uri { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public Stream? Content { get; set; }
}
