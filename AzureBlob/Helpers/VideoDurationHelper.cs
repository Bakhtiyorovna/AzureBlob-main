using NReco.VideoInfo;

namespace AzureBlob1.Helpers;

public class VideoDurationHelper : IVideoDurationHelper
{
    public async Task<double> GetVideoDurationAsync(IFormFile videoFile)
    {
        if (videoFile == null || videoFile.Length == 0)
        {
            // Handle invalid input gracefully.
            return 0; // Or throw an exception or return an error message.
        }

        // Create a temporary file to save the uploaded video.
        var tempFilePath = Path.GetTempFileName();

        using (var stream = new FileStream(tempFilePath, FileMode.Create))
        {
            await videoFile.CopyToAsync(stream);
        }

        // Use NReco.VideoInfo to get the video duration.
        var ffProbe = new FFProbe();
        var videoInfo = ffProbe.GetMediaInfo(tempFilePath);

        // Delete the temporary file.
        File.Delete(tempFilePath);
        
        return videoInfo.Duration.TotalSeconds;
    }
}
