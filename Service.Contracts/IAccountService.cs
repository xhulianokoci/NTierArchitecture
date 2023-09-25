using Entities.Models;

namespace Service.Contracts;

public interface IAccountService
{
    Task<Account> GetAccountById(Guid accountId);
}