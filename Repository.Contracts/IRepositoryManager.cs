namespace Repository.Contracts;

public interface IRepositoryManager
{
    IAccountRepository AccountRepository { get; }
    IUserRepository UserRepository { get; }
    Task SaveAsync();
}
