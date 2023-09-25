using AutoMapper;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Repository.Contracts;
using Service.Contracts;

namespace Service;

public class ServiceManager : IServiceManager
{
    private readonly ILoggerManager _loggerManager;
    private readonly Lazy<IAccountService> _accountService;
    private readonly Lazy<IAuthenticationService> _authenticationService;


    public ServiceManager(IRepositoryManager repositoryManager
        , UserManager<ApplicationUser> userManager
        , ILoggerManager logger
        , IMapper mapper)
    {
        _accountService = new Lazy<IAccountService>(() => new AccountService(repositoryManager));
        _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(logger, userManager, mapper));
    }

    public IAccountService AccountService => _accountService.Value;
    public IAuthenticationService AuthenticationService => _authenticationService.Value;
}
