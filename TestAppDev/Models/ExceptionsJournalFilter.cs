using System.ComponentModel.DataAnnotations.Schema;

namespace TestAppDev.Models;

[NotMapped]
public class ExceptionsJournalFilter
{
    /// <example>2025-05-20T18:02:28.036277Z</example>
    public DateTime? From { get; set; }

    /// <example>2025-05-20T18:02:28.036277Z</example>
    public DateTime? To { get; set; }

    public string? Search {  get; set; }
}
