using Library.API.Domain.Repositories;
using Library.API.Infrastructure.Contexts;

namespace Library.API.Infrastructure.Repositories;

public class UnitOfWork(ApiDbContext context) : IUnitOfWork
{
    public async Task CompleteAsync() => await context.SaveChangesAsync();
}
