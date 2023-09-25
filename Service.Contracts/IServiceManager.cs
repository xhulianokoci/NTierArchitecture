namespace Service.Contracts;

public interface IServiceManager
{
    IAccountService AccountService { get; }
    IAuthenticationService AuthenticationService { get; }
    IUserService UserService { get; }
}
