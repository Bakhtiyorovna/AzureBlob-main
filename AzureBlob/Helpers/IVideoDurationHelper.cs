namespace AzureBlob1.Helpers;

public interface IVideoDurationHelper
{
    public Task<double> GetVideoDurationAsync(IFormFile videoFile);


}
