namespace AzureBlob1.Entities;

public class Video
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string VideoName { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public string VideoImageName { get; set; } = string.Empty;
    public string VideoImageUrl { get; set; } = string.Empty;
    public double VideoDuration {  get; set; }
}
