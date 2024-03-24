using AzureBlob1.Dtos;
using AzureBlob1.Entities;

namespace AzureBlob1.Services;

public interface IVideoService
{
    public Task<bool> UploadAsync(VideoDto video);
    public Task<bool> DownloadAsync(long id);
    public Task<bool> DeleteAsync(long id);
    public Task<Video> GetAsync(long id);
    public Task<IList<Video>> GetAllAsync();
    public Task<IList<Video>>? SearchAsync(string search);
    public Task<bool> UploadVideoAsync(VideoDto dto);
}
