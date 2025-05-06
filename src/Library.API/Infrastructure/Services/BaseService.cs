using Microsoft.EntityFrameworkCore;
using Library.API.Domain.Repositories;

namespace Library.API.Infrastructure.Services;

public abstract class BaseService
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly ILogger _logger;

    protected BaseService(IUnitOfWork unitOfWork, ILogger logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    protected async Task SaveChangesAsync()
    {
        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving changes to database");
            throw new DbUpdateException("Failed to save changes to the database.", ex);
        }
    }
} 