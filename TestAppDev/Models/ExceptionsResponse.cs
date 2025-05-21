using System.ComponentModel.DataAnnotations.Schema;

namespace TestAppDev.Models;

[NotMapped]
public class ExceptionsResponse
{
    public int Skip {  get; set; }

    public int Count { get; set; }

    public IEnumerable<ExceptionItem>? Items { get; set; }
}
