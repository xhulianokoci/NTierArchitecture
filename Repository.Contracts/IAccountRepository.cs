using Entities.Models;

namespace Repository.Contracts;

public interface IAccountRepository
{
    void CreateRecord(Account account);
    void UpdateRecord(Account account);
    void DeleteRecord(Account account);
    Task<Account> GetRecordById(Guid id);
}
