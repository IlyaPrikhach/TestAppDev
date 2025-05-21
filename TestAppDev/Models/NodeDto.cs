using System.ComponentModel.DataAnnotations.Schema;

namespace TestAppDev.Models;

[NotMapped]
public class NodeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid TreeId { get; set; }
    public Guid? ParentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<NodeDto> Children { get; set; } = new();
}
