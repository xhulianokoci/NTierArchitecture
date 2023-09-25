using AutoMapper;
using Entities.Exeptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Repository.Contracts;
using Service.Contracts;
using Shared.DTO;

namespace Service;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILoggerManager _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public AuthenticationService(ILoggerManager logger, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _logger = logger;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<bool> SignUp (CreateUserDTO signUp)
    {
        var foundPhoneNumber = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == signUp.PhoneNumber);
        if (foundPhoneNumber != null && !foundPhoneNumber.UserIsActive)
            throw new BadRequestException("Inactive user already has this phone number!");
        else if (foundPhoneNumber != null && foundPhoneNumber.UserIsActive)
            throw new BadRequestException("Phone number already exists.");

        var foundEmail = _userManager.Users.FirstOrDefault(x => x.Email == signUp.Email);
        if (!string.IsNullOrWhiteSpace(signUp.Email) && foundEmail != null)
            throw new BadRequestException("Email already exists.");

        var user = _mapper.Map<ApplicationUser>(signUp);

        return true;
    }

}
