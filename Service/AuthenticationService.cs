using AutoMapper;
using Cryptography;
using Entities.Exeptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository.Contracts;
using Service.Contracts;
using Shared.DTO;
using Shared.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Service;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILoggerManager _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOptions<JwtConfiguration> _configuration;
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly ICryptoUtils _cryptoUtils;

    public AuthenticationService(ILoggerManager logger, UserManager<ApplicationUser> userManager, IMapper mapper, SignInManager<ApplicationUser> signInManager,
        IOptions<JwtConfiguration> configuration, ICryptoUtils cryptoUtils)
    {
        _logger = logger;
        _userManager = userManager;
        _mapper = mapper;
        _signInManager = signInManager;
        _configuration = configuration;
        _jwtConfiguration = _configuration.Value;
        _cryptoUtils = cryptoUtils;
    }

    public async Task<TokenDTO> ValidateUserAndCreateToken(UserLoginDTO userLogin)
    {
        ApplicationUser currentUser = null;
        if (Helper.IsValidEmail(userLogin.Username))
            currentUser = _userManager.Users.FirstOrDefault(u => u.Email == userLogin.Username || u.UserName == userLogin.Username);
        else
        {
            string correctPhoneNumber = Helper.ValidateAndReturnItalianPhoneNumber(userLogin.Username);
            currentUser = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == correctPhoneNumber);
        }

        if (currentUser == null)
            throw new BadRequestException("User Not Found!");

        if (!currentUser.UserIsActive)
            throw new BadRequestException("User Inactive!");

        var roles = await _userManager.GetRolesAsync(currentUser);
        if (roles is null)
            throw new BadRequestException("No role found for user!");

        var validateUser = await _signInManager.PasswordSignInAsync(currentUser, userLogin.Password, false, lockoutOnFailure: true);

        if (!validateUser.Succeeded)
        {
            _logger.LogWarn(string.Format("Authentication Failed: ", nameof(ValidateUserAndCreateToken)));

            if (validateUser.IsLockedOut)
            {
                //await HandleLockout(currentUser);
                throw new BadRequestException("Multiple attempts for user!");
            }
            throw new BadRequestException("Wrong Email or Password");
        }
        else
        {
            // login with success, save login info
            var tokenDto = await CreateToken(currentUser, true, userLogin.RememberMe);
            return tokenDto;
        }
    }

    public FirstTimeLoginDTO FirstTimeLoginResponse(UserLoginDTO userLogin)
    {
        var currentUser = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == userLogin.Username || u.Email == userLogin.Username || u.UserName == userLogin.Username);
        if (currentUser is null || !currentUser.UserIsActive)
            throw new BadRequestException("User Inactive!");

        var firstTimeUser = new FirstTimeLoginDTO
        {
            FullName = currentUser.FirstName + " " + currentUser.LastName,
            Email = currentUser.Email,
            PhoneNumber = currentUser.PhoneNumber,
            IsFirstTimeLogin = true,
        };
        return firstTimeUser;
    }

    public async Task<(IdentityResult, TokenDTO)> SignUp (CreateUserDTO signUp)
    {
        TokenDTO tokenDto = null;

        var foundPhoneNumber = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == signUp.PhoneNumber);
        if (foundPhoneNumber != null && !foundPhoneNumber.UserIsActive)
            throw new BadRequestException("Inactive user already has this phone number!");
        else if (foundPhoneNumber != null && foundPhoneNumber.UserIsActive)
            throw new BadRequestException("Phone number already exists.");

        var foundEmail = _userManager.Users.FirstOrDefault(x => x.Email == signUp.Email);
        if (!string.IsNullOrWhiteSpace(signUp.Email) && foundEmail != null)
            throw new BadRequestException("Email already exists.");

        var user = _mapper.Map<ApplicationUser>(signUp);

        user.UserName = !string.IsNullOrWhiteSpace(signUp.Email) ? signUp.Email : Guid.NewGuid().ToString();
        user.DateCreated = DateTime.Now;
        user.UserIsActive = true;

        IdentityResult result = null;

        if (!string.IsNullOrWhiteSpace(signUp.Password))
        {
            result = await _userManager.CreateAsync(user, signUp.Password);
        }

        await _userManager.AddToRoleAsync(user, "Manager");

        return (result, tokenDto);
    }

    public async Task<IdentityResult> ResetPassword(ResetPasswordDTO resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user is null)
            throw new NotFoundException(string.Format("There is no user with this email: ", resetPasswordDto.Email));


        if (resetPasswordDto.Password != resetPasswordDto.ConfirmPassword)
            throw new BadRequestException("Password and Confirm Password doesnt match");

        var resetPassword = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);

        if (!resetPassword.Succeeded)
            throw new BadRequestException("Password didnt change!");

        await _userManager.UpdateAsync(user);

        return resetPassword;
    }

    #region Private Methods

    private async Task<TokenDTO> CreateToken(ApplicationUser? currentUser, bool populateExp, bool rememberMe)
    {
        if (currentUser is not null)
        {
            var signingCredentials = GetSigningCredentials();
            var tokenHash = _cryptoUtils.Encrypt($"{currentUser.Id}{currentUser.Email}{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}");

            var claims = await GetClaims(currentUser, tokenHash);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                NotBefore = DateTime.Now,
                IssuedAt = DateTime.Now,
                Expires = rememberMe ? DateTime.Now.AddMonths(1) : DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
                SigningCredentials = signingCredentials,
                Audience = _jwtConfiguration.ValidAudience,
                Issuer = _jwtConfiguration.ValidIssuer
            };

            var refreshToken = GenerateRefreshToken();
            currentUser.RefreshToken = refreshToken;
            currentUser.TokenHash = tokenHash;

            if (populateExp)
                currentUser.RefreshTokenExpiryTime = rememberMe ? DateTime.Now.AddMonths(2) : DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.RefreshTokenExpire));

            await _userManager.UpdateAsync(currentUser);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            return new TokenDTO(accessToken, refreshToken);
        }

        return new TokenDTO("", "");
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey);
        if (key is null)
            throw new NotFoundException("No key found");

        byte[] secretKey = null;
        using (var sha256 = new SHA256Managed())
        {
            secretKey = sha256.ComputeHash(key);
        }

        var secret = new SymmetricSecurityKey(secretKey);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256Signature);
    }

    private async Task<ClaimsIdentity> GetClaims(ApplicationUser currentUser, string tokenHash)
    {
        string managerName = "";

        var claims = new List<Claim>
             {
                new Claim("Id", currentUser.Id.ToString()),
                new Claim("Email", !string.IsNullOrWhiteSpace(currentUser.Email)?currentUser.Email : ""),
                new Claim("PhoneNumber", currentUser.PhoneNumber !=null?currentUser.PhoneNumber  :""),
                new Claim("FirstName", !string.IsNullOrWhiteSpace(currentUser.FirstName)?currentUser.FirstName : ""),
                new Claim("LastName", !string.IsNullOrWhiteSpace(currentUser.LastName)? currentUser.LastName : ""),
                new Claim("TokenHash", tokenHash),
                //new Claim("Image", currentUser != null && !string.IsNullOrWhiteSpace(currentUser.Image) ? $"{_defaultConfig.APIUrl}{currentUser.Image}" : ""),
        };

        var roles = await _userManager.GetRolesAsync(currentUser);
        if (roles is null)
            throw new NotFoundException(string.Format("No role found for user with id: ", currentUser.Id));

        if (roles.Count == 1)
        {
            claims.Add(new Claim(ClaimTypes.Role, roles[0]));

            if (roles[0] == UserRole.Admin)
                claims.Add(new Claim("DefaultRoute", "/admin/dashboard"));

            if (roles[0] == UserRole.Manager)
                claims.Add(new Claim("DefaultRoute", "/manager/calendar"));
        }
        else if (roles.Count > 1)
        {
            var teamMemberManagerRole = roles.FirstOrDefault(x => x == UserRole.Manager);
        }

        return new ClaimsIdentity(claims);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    #endregion

}
