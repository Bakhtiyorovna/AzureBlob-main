using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlob1.Dtos;
using AzureBlob1.Entities;
using AzureBlob1.Exceptions;
using AzureBlob1.Helpers;
using AzureBlob1.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AzureBlob1.Services;

public class VideoService : IVideoService
{

    private readonly string _storageAccount = "databaza1234567";
    private readonly string _key = "DKscsabD5DCSvNySR9XDP87qouUGVti5j4CNst8LQNavQt4eCS60RHP5huQ8q66fCNtAVFhHQKrH+AStNBOiWw==";

    private readonly BlobContainerClient _filesContainer;
    private readonly IVideoDurationHelper _duration;
    private readonly IRepository<Video> _repository;

    //wwwRoot
    private readonly string MEDIA = "media";
    private readonly string IMAGES = "images";
    private readonly string VIDEOS = "videos";
    private readonly string ROOTHPATH;

    public VideoService(IVideoDurationHelper duration,
                        IRepository<Video> repository, IWebHostEnvironment env)
    {
        var credential = new StorageSharedKeyCredential(_storageAccount, _key);
        var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
        var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        _filesContainer = blobServiceClient.GetBlobContainerClient("container");
        _duration = duration;
        _repository = repository;
        ROOTHPATH = env.WebRootPath;
    }


    public async Task<bool> DeleteAsync(long id)
    {
        var video = await _repository.SelectAsync(x => x.Id == id);
        if (video is null) throw new VideoNotFoundException();
        
        BlobClient file = _filesContainer.GetBlobClient(video.VideoName);
        await file.DeleteAsync();
        file = _filesContainer.GetBlobClient(video.VideoImageName);
        await file.DeleteAsync();

        var result = await _repository.DeleteAsync(x => x.Id == id);
                     await _repository.SaveAsync();
        
        return result;
    }


    public Task<bool> DownloadAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task<IList<Video>> GetAllAsync()
    {
        var videos = await _repository.SelectAll().ToListAsync();
        if (videos is null) throw new VideoNotFoundException();
        return videos;
    }

    public async Task<Video> GetAsync(long id)
    {
        var video = await _repository.SelectAsync(x=> x.Id == id);
        if (video is null) throw new VideoNotFoundException();
        return video;

    }

    public async Task<IList<Video>> SearchAsync(string search)
    {
        var lower = search.ToLower();
        var videos = await _repository.SelectAll(x => x.Title.ToLower().Contains(lower)).ToListAsync();
        
        return videos;
    }
    public async Task<bool> UploadVideoAsync(VideoDto dto)
    {
        string directoryPath = "wwwroot/media/names";
        string fileName = dto.Title + "___" + Guid.NewGuid().ToString();
        string filePath = Path.Combine(directoryPath, fileName);
        var nom = File.Create(filePath);
        nom.Dispose();
        nom.Close();

        FileInfo VideoInfo = new FileInfo(dto.Video.FileName);
        string newVideoName = fileName + VideoInfo.Extension;
        string subPathVideo = Path.Combine(MEDIA, VIDEOS, newVideoName);
        string path = Path.Combine(ROOTHPATH, subPathVideo);

        var stream = new FileStream(path, FileMode.Create);
        await dto.Video.CopyToAsync(stream);
        stream.Dispose();
        stream.Close();


        FileInfo ImageInfo = new FileInfo(dto.VideoImage.FileName);
        string newImageName = fileName + ImageInfo.Extension;
        string subPathImage = Path.Combine(MEDIA, IMAGES, newImageName);
        path = Path.Combine(ROOTHPATH, subPathImage);
        
        var streamImage = new FileStream(path, FileMode.Create);
        await dto.VideoImage.CopyToAsync(streamImage);
        streamImage.Dispose();
        streamImage.Close();

    
        return true;
    }

    public async Task<bool> UploadAsync(VideoDto dto)
    {
        Video video = new Video();

        string videoName = MediaHelper.MakeVideoName(dto.Video.FileName);
        string imageName = MediaHelper.MakeImageName(dto.VideoImage.FileName);

        // Specify the content type for the video file
        BlobUploadOptions videoUploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = dto.Video.ContentType 
            }
        };

        BlobClient videoUpload = _filesContainer.GetBlobClient(videoName);
        await using (Stream streamVideo = dto.Video.OpenReadStream())
        {
            await videoUpload.UploadAsync(streamVideo, videoUploadOptions);
        }

        // Specify the content type for the image file
        BlobUploadOptions imageUploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = dto.VideoImage.ContentType // Set the content type based on the uploaded file's content type
            }
        };

        BlobClient imageUpload = _filesContainer.GetBlobClient(imageName);
        await using (Stream streamImage = dto.VideoImage.OpenReadStream())
        {
            await imageUpload.UploadAsync(streamImage, imageUploadOptions);
        }

        video.Title = dto.Title;
        video.VideoDuration = await _duration.GetVideoDurationAsync(dto.Video);
        video.VideoUrl = videoUpload.Uri.AbsoluteUri;
        video.VideoName = videoName;
        video.VideoImageUrl = imageUpload.Uri.AbsoluteUri;
        video.VideoImageName = imageName;

        await _repository.AddAsync(video);
        await _repository.SaveAsync();

        return true;
    }

}
