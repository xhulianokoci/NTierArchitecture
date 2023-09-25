using Shared.DTO;

namespace Service.Contracts;

public interface IUserService
{
    Task<UserListDTO> GetRecordById(int userId);
}
