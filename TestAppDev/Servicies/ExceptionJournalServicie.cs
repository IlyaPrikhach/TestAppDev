using Microsoft.EntityFrameworkCore;
using TestAppDev.Data;
using TestAppDev.Interfaces;
using TestAppDev.Models;

public class ExceptionJournalService: IExceptionJournalService
{
    private readonly AppDbContext _context;

    public ExceptionJournalService(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveExceptionToJournalAsync(ExceptionJournal exceptionJournal)
    {
        _context.ExceptionJournals.Add(exceptionJournal);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ExceptionItem>> GetExceptionItemsAsync(int skip, int take, ExceptionsJournalFilter filter)
    {
        var query = _context.ExceptionJournals.AsQueryable();

        if (filter.From.HasValue)
        {
            query = query.Where(x => x.Timestamp >= filter.From.Value);
        }

        if (filter.To.HasValue)
        {
            query = query.Where(x => x.Timestamp <= filter.To.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchTerm = filter.Search.Trim().ToLower();
            query = query.Where(x =>
                x.ExceptionMessage.ToLower().Contains(searchTerm) ||
                x.ExceptionType.ToLower().Contains(searchTerm) ||
                (x.QueryParameters != null && x.QueryParameters.ToLower().Contains(searchTerm)) ||
                (x.BodyParameters != null && x.BodyParameters.ToLower().Contains(searchTerm)));
        }

        query = query.OrderByDescending(x => x.Timestamp);

        return await query
            .Skip(skip)
            .Take(take)
            .Select(x => new ExceptionItem
            {
                Id = x.Id,
                EventId = x.EventId,
                Text = x.ExceptionMessage,
            })
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ExceptionItem?> GetExceptionItemAsync(Guid id)
    {

        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty", nameof(id));
        }

        return await _context.ExceptionJournals
            .Where(x => x.Id == id)
            .Select(x => new ExceptionItem
            {
                Id = x.Id,
                EventId = x.EventId,
                Text = x.ExceptionMessage
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
}
