using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Service.Contracts;

namespace NTierTemplate.Presantation.Resources.Controllers;

[Route("api/user")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IServiceManager _service;

    public UserController(IServiceManager service)
    {
        _service = service;
    }

    [HttpGet()]
    public async Task<IActionResult> GetUserById()
    {
        var result = await _service.UserService.GetRecordById(int.Parse(User.Claims.First(c => c.Type == "Id").Value));
        return Ok(result);
    }
}