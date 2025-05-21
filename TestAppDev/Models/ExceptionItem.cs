using System.ComponentModel.DataAnnotations.Schema;

namespace TestAppDev.Models;

[NotMapped]
public class ExceptionItem
{
    public Guid Id { get; set; }

    public string EventId { get; set; } = Guid.NewGuid().ToString();

    public DateTime CreatedAt { get; set; }

    public string Text { get; set;}
}
