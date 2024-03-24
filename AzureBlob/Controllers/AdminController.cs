using AzureBlob1.Dtos;
using AzureBlob1.Services;
using AzureBlob1.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlob1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Get([FromBody] LoginDto loginDto)
    {
        var validator = new LoginValidator();
        var check = validator.Validate(loginDto);
        if(check.IsValid)
        {
            var result = await _adminService.LoginAsync(loginDto);
            return Ok(new {result.Result , result.Token});
        }
        else
        {
            return BadRequest(check.Errors);
        }
    }


    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromForm] AdminCreateDto dto)
    {
        var result = await _adminService.CreateAsync(dto);
        return Ok(result);
    }
    
}
