namespace TestAppDev.Data;

public class Node
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid TreeId { get; set; }
    public Guid? ParentId { get; set; }
    public Node? Parent { get; set; }
    public ICollection<Node> Children { get; set; } = new List<Node>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
