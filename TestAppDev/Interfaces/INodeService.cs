using TestAppDev.Data;
using TestAppDev.Models;
namespace TestAppDev.Interfaces;

public interface INodeService
{
    Task<IEnumerable<NodeDto>> GetOrCreateTreeNodesAsync(string treeName);

    Task<Node> CreateNodeAsync(string treeName, Guid? parentNodeId, string nodeName);

    Task DeleteNodeAsync(string treeName, Guid nodeId);

    Task<Node> RenameNodeAsync(string treeName, Guid nodeId, string noewNodeName);
}
