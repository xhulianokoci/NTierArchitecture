using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace NTierTemplate.Presantation.Resources.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _service;

    public AuthenticationController(IServiceManager service)
    {
        _service = service;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] CreateUserDTO signUp)
    {
        var user = await _service.AuthenticationService.SignUp(signUp);

        return Ok(user);
    }
}
