using Microsoft.AspNetCore.Identity;
using Shared.DTO;

namespace Service.Contracts;

public interface IAuthenticationService
{
    Task<(IdentityResult, TokenDTO)> SignUp(CreateUserDTO signUp);
    Task<TokenDTO> ValidateUserAndCreateToken(UserLoginDTO userLogin);
    FirstTimeLoginDTO FirstTimeLoginResponse(UserLoginDTO userLogin);
    Task<IdentityResult> ResetPassword(ResetPasswordDTO resetPasswordDto);
}
