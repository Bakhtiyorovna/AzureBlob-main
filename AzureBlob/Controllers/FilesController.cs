using AzureBlob1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlob1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly FileService _fileService;

        public FilesController(FileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> ListAllblobs()
        {
            var result = await _fileService.ListAsync();
            return Ok(result);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }


            var response = await _fileService.UploadAsync(file);

            if (response.Error)
            {
                return BadRequest(response.Status);
            }

            return Ok(response);
        }

  
        [HttpGet]
        [Route("filename")]
        public async Task<IActionResult> Download(string filename)
        {
            var result = await _fileService.DownloadAsync(filename);

            if (result != null)
            {
                // Return the blob content as a file.
                return File(result, "application/octet-stream", filename);
            }
            else
            {
                // Handle the case where the blob doesn't exist, perhaps return a not found response.
                return NotFound("Blob not found");
            }
        }


        [HttpDelete]
        [Route("filename")]
        public async Task<IActionResult> Delete(string filename)
        {
            var result = await _fileService.DeleteAsync(filename);
            return Ok(result);
        }
    }
}
