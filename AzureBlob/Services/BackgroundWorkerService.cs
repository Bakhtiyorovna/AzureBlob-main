
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlob1.Entities;
using AzureBlob1.Helpers;
using AzureBlob1.Repositories;

namespace AzureBlob1.Services
{
    public class BackgroundWorkerService : BackgroundService
    {
        private readonly ILogger<BackgroundWorkerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly BlobContainerClient _filesContainer;


        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger,

                                        IServiceProvider serviceProvider)
        {
            _logger = logger;

            _serviceProvider = serviceProvider;

            var _storageAccount = "databaza1234567";
            var _key = "DKscsabD5DCSvNySR9XDP87qouUGVti5j4CNst8LQNavQt4eCS60RHP5huQ8q66fCNtAVFhHQKrH+AStNBOiWw==";

            var credential = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
                
            _filesContainer = blobServiceClient.GetBlobContainerClient("container");
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Log a message to indicate that the service is running
                    await Console.Out.WriteLineAsync("Service is running.");

                    string[] files = Directory.GetFiles("wwwroot/media/videos");
                    string[] names = Directory.GetFiles("wwwroot/media/names");
                    string[] images = Directory.GetFiles("wwwroot/media/images");

                    if (names != null && names.Length > 0)
                    {
                        foreach (string name in names)
                        {
                            using (IServiceScope scope = _serviceProvider.CreateScope())
                            {
                                var _repository = scope.ServiceProvider.GetRequiredService<IRepository<Video>>();

                                Video saveVideo = new Video();

                                string clearName = Path.GetFileName(name);
                                saveVideo.Title = clearName.Split(new[] { "___" }, StringSplitOptions.None).First();

                                // Uploading videos
                                string[] VideosWithName = files
                                   .Where(filePath => Path.GetFileName(filePath).Contains(clearName))
                                   .ToArray();
                                foreach (string file in VideosWithName)
                                {
                                    // Process each file

                                    var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                                    var video = new FormFile(baseStream: fileStream, baseStreamOffset: 0,
                                        length: new FileInfo(file).Length, name: Guid.NewGuid().ToString(), fileName: Path.GetFileName(file));


                                    if(OperatingSystem.IsWindows())
                                    {
                                        var _videoDuration = scope.ServiceProvider.GetRequiredService<IVideoDurationHelper>();
                                        saveVideo.VideoDuration = await _videoDuration.GetVideoDurationAsync(video);
                                    }
                                    else saveVideo.VideoDuration = 0;


                                    saveVideo.VideoName = video.FileName;
                                    string contentType = "video/mp4";
                                    BlobUploadOptions videoUploadOptions = new BlobUploadOptions
                                    {
                                        HttpHeaders = new BlobHttpHeaders
                                        {
                                            ContentType = contentType
                                        }
                                    };
                                    // Upload the file to Azure Blob Storage
                                    BlobClient videoUpload = _filesContainer.GetBlobClient(Path.GetFileName(file));
                                    
                                    await using (Stream streamVideo = video.OpenReadStream())
                                    {
                                        
                                        await videoUpload.UploadAsync(streamVideo, videoUploadOptions);
                                        
                                        streamVideo.Dispose();
                                        streamVideo.Close();
                                    }

                                    saveVideo.VideoUrl = videoUpload.Uri.AbsoluteUri;

                                    fileStream.Dispose();
                                    fileStream.Close();
                                    // Delete the file from the local directory
                                    File.Delete(file);
                                }

                                // Uploading Images
                                string[] ImagesWithName = images
                                  .Where(filePath => Path.GetFileName(filePath).Contains(clearName))
                                  .ToArray();

                                foreach (string file in ImagesWithName)
                                {
                                    // Process each file
                                    var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                                    var image = new FormFile(baseStream: fileStream, baseStreamOffset: 0,
                                        length: new FileInfo(file).Length, name: Guid.NewGuid().ToString(), fileName: Path.GetFileName(file));

                                    //BlobUploadOptions imageUploadOptions = new BlobUploadOptions
                                    //{
                                    //    HttpHeaders = new BlobHttpHeaders
                                    //    {
                                    //        ContentType = image.ContentType
                                    //    }
                                    //};

                                    // Upload the file to Azure Blob Storage
                                    BlobClient imageUpload = _filesContainer.GetBlobClient(Path.GetFileName(file));
                                    await using (Stream streamImage = image.OpenReadStream())
                                    {
                                        await imageUpload.UploadAsync(streamImage);

                                        streamImage.Dispose();
                                        streamImage.Close();
                                    }
                                    saveVideo.VideoImageName = image.FileName;
                                    saveVideo.VideoImageUrl = imageUpload.Uri.AbsoluteUri;

                                    fileStream.Dispose();
                                    fileStream.Close();
                                    // Delete the file from the local directory
                                    File.Delete(file);
                                }

                                File.Delete(name);
                                await _repository.AddAsync(saveVideo);
                                await _repository.SaveAsync();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log and handle any exceptions that occur during processing
                    _logger.LogError(ex, "An error occurred while processing a file.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}