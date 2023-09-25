using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts;

namespace Repository;

public class AccountRepository :RepositoryBase<Account>, IAccountRepository
{
    public AccountRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

    public void CreateRecord(Account billing) => Create(billing);

    public void UpdateRecord(Account billing) => Update(billing);

    public void DeleteRecord(Account billing) => Update(billing);

    public async Task<Account> GetRecordById(Guid id) =>
      await FindByCondition(x => x.Id.Equals(id)).SingleOrDefaultAsync();
}
