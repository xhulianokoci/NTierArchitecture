using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Contracts;

namespace NTierTemplate.Presantation.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IServiceManager _service;

    public AccountController(ILogger<AccountController> logger, IServiceManager service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet("get/{accountId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAccount(Guid accountId)
    {
        var result = await _service.AccountService.GetAccountById(accountId);
        return Ok(result);
    }
}