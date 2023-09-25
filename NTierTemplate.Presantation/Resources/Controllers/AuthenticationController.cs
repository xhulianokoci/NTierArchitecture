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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO userLogin)
    {
        var tokenDto = await _service.AuthenticationService.ValidateUserAndCreateToken(userLogin);
        return Ok(tokenDto);
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] CreateUserDTO signUp)
    {
        (IdentityResult result, TokenDTO tokenDto) = await _service.AuthenticationService.SignUp(signUp);

        if (tokenDto != null)
            return Ok(tokenDto);

        return StatusCode(201);
    }
}
