using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TestAppDev.Interfaces;
using TestAppDev.Models;

namespace TestAppDev.Controllers;
/// <summary>
/// Represents journal API
/// </summary>
[SwaggerTag("user.journal")]
[ApiController]
public class ExceptionJournalController : ControllerBase
{
    private readonly IExceptionJournalService _journalService;

    public ExceptionJournalController(IExceptionJournalService journalService)
    {
        _journalService = journalService;
    }

    /// <summary>
    /// Provides the pagination API. Skip means the number of items should be skipped by server. 
    /// Take means the maximum number items should be returned by server. All fields of the filter are optional.
    /// </summary>
    [Route("/api.user.journal.getRange")]
    [HttpPost]
    public async Task<ActionResult<ExceptionsResponse>> GetRange(
        [FromQuery] int skip,
        [FromQuery] int take,
        [FromBody] ExceptionsJournalFilter filter)
    {
        var exceptionJournals = await _journalService.GetExceptionItemsAsync(skip, take, filter);
        var response = new ExceptionsResponse
        {
            Skip = skip,
            Count = exceptionJournals.Count(),
            Items = exceptionJournals
        };
        return Ok(response);
    }

    /// <summary>
    /// Returns the information about an particular event by ID.
    /// </summary>
    [HttpGet("/api.user.journal.getSingle")]
    public async Task<ActionResult<ExceptionItem>> GetSingle([FromQuery] Guid id)
    {
        var response = await _journalService.GetExceptionItemAsync(id);
        return Ok(response);
    }
}
