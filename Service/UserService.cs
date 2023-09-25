using AutoMapper;
using Entities.Exeptions;
using Entities.Models;
using Repository.Contracts;
using Service.Contracts;
using Shared.DTO;
using Shared.Utility;

namespace Service;

public class UserService : IUserService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IMapper _mapper;
    private readonly DefaultConfiguration _defaultConfig;

    public UserService(IRepositoryManager repositoryManager, IMapper mapper, DefaultConfiguration defaultConfig)
    {
        _repositoryManager = repositoryManager;
        _mapper = mapper;
        _defaultConfig = defaultConfig;
    }

    public async Task<UserListDTO> GetRecordById(int userId)
    {
        var existingUser = await GetUserAndCheckIfExistsAsync(userId);

        var userById = _mapper.Map<UserListDTO>(existingUser);
        userById.Image = !string.IsNullOrWhiteSpace(existingUser.Image) ? $"{_defaultConfig.APIUrl}{existingUser.Image}" : null;

        return userById;
    }

    #region Private Methods

    private async Task<ApplicationUser> GetUserAndCheckIfExistsAsync(int userId)
    {
        var existingUser = await _repositoryManager.UserRepository.GetRecordByIdAsync(userId);
        if (existingUser is null)
            throw new NotFoundException(string.Format("No user with id: ", userId));

        return existingUser;
    }

    #endregion
}
