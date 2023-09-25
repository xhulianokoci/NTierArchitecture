using Repository.Contracts;

namespace Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IAccountRepository> _accountRepository;
    private readonly Lazy<IUserRepository> _userRepository;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _accountRepository = new Lazy<IAccountRepository>(() => new AccountRepository(repositoryContext));
        _userRepository = new Lazy<IUserRepository>(() => new UserRepository(repositoryContext));
    }

    public IAccountRepository AccountRepository => _accountRepository.Value;
    public IUserRepository UserRepository => _userRepository.Value;

    public async Task SaveAsync()
    {
        _repositoryContext.ChangeTracker.AutoDetectChangesEnabled = false;
        await _repositoryContext.SaveChangesAsync();
    }
}
