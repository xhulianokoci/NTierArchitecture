using Shared.DTO;

namespace Service.Contracts;

public interface IAuthenticationService
{
    Task<bool> SignUp(CreateUserDTO signUp);
}
