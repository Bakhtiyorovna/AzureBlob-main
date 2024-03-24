using AzureBlob1.Dtos;
using AzureBlob1.Services;
using AzureBlob1.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlob1.Controllers;

[Route("api/Video")]
[ApiController]
public class VideoController : ControllerBase
{
    private readonly IVideoService _service;
    

    public VideoController(IVideoService service)
    {
        _service = service;
    }


    [HttpGet("all")]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("one/{id}")]
    public async Task<IActionResult> GetOneAsync(long id)
    {
        var result = await _service.GetAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromForm] VideoDto dto)
    {
        var validator = new VideoValidator();
        var check = validator.Validate(dto);
        if (check.IsValid)
        {
            var result = await _service.UploadVideoAsync(dto);
            return Ok(result);
        }
        else return BadRequest(check.Errors);    
    }

    

    [HttpPost("search")]
    public async Task<IActionResult> SearchAsync([FromForm] string search)
    {   
        var result = await _service.SearchAsync(search);
        return Ok(result);  
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var result = await _service.DeleteAsync(id);
        return Ok(result);  
    }


}
