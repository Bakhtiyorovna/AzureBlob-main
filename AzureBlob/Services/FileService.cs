using Azure.Storage;
using Azure.Storage.Blobs;
using AzureBlob1.Dtos;

namespace AzureBlob1.Services;

public class FileService
{
    private readonly string _storageAccount = "databaza1234567";
    private readonly string _key = "DKscsabD5DCSvNySR9XDP87qouUGVti5j4CNst8LQNavQt4eCS60RHP5huQ8q66fCNtAVFhHQKrH+AStNBOiWw==";


    private readonly BlobContainerClient _filesContainer;

    public FileService()
    {
        var credential = new StorageSharedKeyCredential(_storageAccount, _key);
        var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
        var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        _filesContainer = blobServiceClient.GetBlobContainerClient("container1");
    }

    public async Task<List<BlobDto>> ListAsync()
    {
        List<BlobDto> files = new List<BlobDto>();
        await foreach (var file in _filesContainer.GetBlobsAsync())
        {
            string uri = _filesContainer.Uri.ToString();
            var name = file.Name;
            var fullUri = $"{uri}/{name}";

            files.Add(new BlobDto
            {
                Uri = fullUri,
                Name = name,
                ContentType = file.Properties.ContentType
            });
        }
        return files;
    }

    public async Task<BlobResponseDto> UploadAsync(IFormFile blob)
    {
        BlobResponseDto response = new();
        Random random = new Random();
        int number = random.Next(1000, 9999);
        BlobClient client = _filesContainer.GetBlobClient("file_"+ number.ToString());

        await using (Stream data = blob.OpenReadStream())
        {
            await client.UploadAsync(data);
        }

        response.Status = $"File {blob.FileName} Uploaded Successfully";
        response.Error = false;
        response.Blob.Uri = client.Uri.AbsoluteUri;
        response.Blob.Name = client.Name;

        return response;
    }

    private string Guid()
    {
        throw new NotImplementedException();
    }

    public async Task<Stream?> DownloadAsync(string blobFilename)
    {
        BlobClient file = _filesContainer.GetBlobClient(blobFilename);

        if (await file.ExistsAsync())
        {
            var data = await file.OpenReadAsync();
            Stream blobContent = data;

            var content = await file.DownloadContentAsync();

            return blobContent;
        }

        return null;
    }


    public async Task<BlobResponseDto> DeleteAsync(string blobFilename)
    {
        BlobClient file = _filesContainer.GetBlobClient(blobFilename);

        await file.DeleteAsync();

        return new BlobResponseDto
        {
            Error = false,
            Status = $"File: {blobFilename} has been successfully  deleted."
        };
    }


}
