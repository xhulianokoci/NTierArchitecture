namespace Service.Contracts;

public interface IServiceManager
{
    IAccountService AccountService { get; }
    IAuthenticationService AuthenticationService { get; }
}
