using System.ComponentModel.DataAnnotations.Schema;
using TestAppDev.Data;

namespace TestAppDev.Models;

[NotMapped]
public class TreeResponse
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public ICollection<NodeDto> Children { get; set; } = new List<NodeDto>();
}
