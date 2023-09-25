using Entities.Models;
using Repository.Contracts;
using Service.Contracts;

namespace Service;

public class AccountService : IAccountService
{
    private readonly IRepositoryManager _repositoryManager;

    public AccountService(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    public async Task<Account> GetAccountById(Guid accountId)
    {
        var accountToReturn = await _repositoryManager.AccountRepository.GetRecordById(accountId);

        return accountToReturn;
    }
}