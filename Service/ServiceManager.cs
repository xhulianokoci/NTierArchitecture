using AutoMapper;
using Cryptography;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Repository.Contracts;
using Service.Contracts;
using Shared.Utility;

namespace Service;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAccountService> _accountService;
    private readonly Lazy<IAuthenticationService> _authenticationService;
    private readonly Lazy<IUserService> _userService;


    public ServiceManager(IRepositoryManager repositoryManager
        , UserManager<ApplicationUser> userManager
        , DefaultConfiguration defaultConfig
        , ILoggerManager logger
        , IMapper mapper
        , SignInManager<ApplicationUser> signInManager
        , IOptions<JwtConfiguration> jwtConfiguration
        , ICryptoUtils cryptoUtils)
    {
        _accountService = new Lazy<IAccountService>(() => new AccountService(repositoryManager));
        _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(logger, userManager, mapper, signInManager, jwtConfiguration, cryptoUtils));
        _userService = new Lazy<IUserService>(() => new UserService(repositoryManager, mapper, defaultConfig));
    }

    public IAccountService AccountService => _accountService.Value;
    public IAuthenticationService AuthenticationService => _authenticationService.Value;
    public IUserService UserService => _userService.Value;
}
