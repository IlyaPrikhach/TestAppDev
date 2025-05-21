using Microsoft.EntityFrameworkCore;
using Npgsql;
using TestAppDev.Data;
using TestAppDev.Interfaces;
using TestAppDev.Models;

namespace TestAppDev.Servicies;

public class NodeService : INodeService
{
    private readonly AppDbContext _context;

    public NodeService(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NodeDto>> GetOrCreateTreeNodesAsync(string treeName)
    {
        var nodes = await _context.Nodes
            .Where(n => n.Name == treeName && n.ParentId == null)
            .Include(n => n.Children)
            .ToListAsync();

        if (!nodes.Any())
        {
            var newRoot = new Node
            {
                Id = Guid.NewGuid(),
                Name = treeName,
                TreeId = Guid.NewGuid(),
                ParentId = null,
                CreatedAt = DateTime.UtcNow
            };

            _context.Nodes.Add(newRoot);
            await _context.SaveChangesAsync();
            nodes = new List<Node> { newRoot };
        }

        return nodes.Select(MapToDto);
    }

    public async Task<Node> CreateNodeAsync(string treeName, Guid? parentNodeId, string nodeName)
    {
        if (string.IsNullOrWhiteSpace(treeName))
            throw new ArgumentException("Tree name cannot be empty", nameof(treeName));

        if (string.IsNullOrWhiteSpace(nodeName))
            throw new ArgumentException("Node name cannot be empty", nameof(nodeName));

        Node? parentNode = null;
        if (parentNodeId.HasValue)
        {
            parentNode = await _context.Nodes
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == parentNodeId.Value && n.Name == treeName);

            if (parentNode == null)
                throw new KeyNotFoundException($"Parent node with id {parentNodeId} not found in tree '{treeName}'");
        }
        else
        {
            bool rootExists = await _context.Nodes
                .AsNoTracking()
                .AnyAsync(n => n.Name == treeName && n.ParentId == null);

            if (rootExists)
                throw new SecureException($"Tree '{treeName}' already has a root node");
        }

        bool nameExists = await _context.Nodes
            .AsNoTracking()
            .AnyAsync(n => n.Name == nodeName &&
                          n.ParentId == parentNodeId &&
                          n.Name == treeName);

        if (nameExists)
            throw new SecureException($"Node name '{nodeName}' already exists among siblings in tree '{treeName}'");

        var newNode = new Node
        {
            Id = Guid.NewGuid(),
            Name = nodeName,
            TreeId = parentNode?.TreeId ?? Guid.NewGuid(),
            ParentId = parentNodeId,
            CreatedAt = DateTime.UtcNow
        };

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Nodes.Add(newNode);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23503" })
        {
            throw new InvalidOperationException("Invalid parent node reference", ex);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return newNode;
    }

    public async Task DeleteNodeAsync(string treeName, Guid nodeId)
    {
        if (string.IsNullOrWhiteSpace(treeName))
            throw new ArgumentException("Tree name cannot be empty", nameof(treeName));

        if (nodeId == Guid.Empty)
            throw new ArgumentException("Node ID cannot be empty", nameof(nodeId));

        var nodeToDelete = await _context.Nodes
            .FirstOrDefaultAsync(n => n.Id == nodeId && n.Name == treeName);

        if (nodeToDelete == null)
            throw new KeyNotFoundException($"Node with ID {nodeId} not found in tree '{treeName}'");

        if (nodeToDelete.ParentId == null)
            throw new SecureException("Cannot delete root node. Delete the entire tree instead.");

        var allChildNodes = await GetDescendantsAsync(nodeId);
        var nodesToDelete = allChildNodes.Append(nodeToDelete);

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Nodes.RemoveRange(nodesToDelete);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Node> RenameNodeAsync(string treeName, Guid nodeId, string newNodeName)
    {
        if (string.IsNullOrWhiteSpace(treeName))
            throw new ArgumentException("Tree name cannot be empty", nameof(treeName));

        if (nodeId == Guid.Empty)
            throw new ArgumentException("Node ID cannot be empty", nameof(nodeId));

        if (string.IsNullOrWhiteSpace(newNodeName))
            throw new ArgumentException("New node name cannot be empty", nameof(newNodeName));

        var nodeToRename = await _context.Nodes
            .FirstOrDefaultAsync(n => n.Id == nodeId && n.Name == treeName);

        if (nodeToRename == null)
            throw new KeyNotFoundException($"Node with ID {nodeId} not found in tree '{treeName}'");

        if (string.Equals(nodeToRename.Name, newNodeName, StringComparison.Ordinal))
            return nodeToRename;

        var siblingsQuery = _context.Nodes
            .Where(n => n.ParentId == nodeToRename.ParentId &&
                       n.Id != nodeId && 
                       n.Name == treeName); 

        var nameExists = await siblingsQuery
            .AnyAsync(n => n.Name == newNodeName);

        if (nameExists)
            throw new SecureException($"Node name '{newNodeName}' already exists among siblings");

        nodeToRename.Name = newNodeName;
        nodeToRename.UpdatedAt = DateTime.UtcNow;

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Nodes.Update(nodeToRename);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return nodeToRename;
    }

    private async Task<IEnumerable<Node>> GetDescendantsAsync(Guid parentId)
    {
        var descendants = new List<Node>();
        var directChildren = await _context.Nodes
            .Where(n => n.ParentId == parentId)
            .ToListAsync();

        foreach (var child in directChildren)
        {
            descendants.Add(child);
            descendants.AddRange(await GetDescendantsAsync(child.Id));
        }

        return descendants;
    }

    private NodeDto MapToDto(Node node)
    {
        return new NodeDto
        {
            Id = node.Id,
            Name = node.Name,
            TreeId = node.TreeId,
            ParentId = node.ParentId,
            CreatedAt = node.CreatedAt,
            Children = node.Children.Select(MapToDto).ToList()
        };
    }
}
