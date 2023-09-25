namespace Repository.Contracts;

public interface IRepositoryManager
{
    IAccountRepository AccountRepository { get; }
    Task SaveAsync();
}
