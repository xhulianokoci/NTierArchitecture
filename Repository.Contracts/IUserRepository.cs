using Entities.Models;

namespace Repository.Contracts;

public interface IUserRepository
{
    Task<ApplicationUser> GetRecordByIdAsync(int id);
    void UpdateRecord(ApplicationUser applicationUser);
    void DeleteRecord(ApplicationUser user);
    Task<IEnumerable<ApplicationUser>> GetAllRecordsAsync();
    Task<ApplicationUser> GetUserByPhoneNumber(string phoneNumber);
}
