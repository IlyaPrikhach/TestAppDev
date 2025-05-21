using TestAppDev.Models;

namespace TestAppDev.Interfaces;

public interface IExceptionJournalService
{
    Task SaveExceptionToJournalAsync(ExceptionJournal exceptionJournal);

    Task<IEnumerable<ExceptionItem>> GetExceptionItemsAsync(int skip, int take, ExceptionsJournalFilter filter);

    Task<ExceptionItem?> GetExceptionItemAsync(Guid id);
}
