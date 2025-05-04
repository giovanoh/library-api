namespace Library.API.Domain.Repositories;

public interface IUnitOfWork
{
     Task CompleteAsync();
}