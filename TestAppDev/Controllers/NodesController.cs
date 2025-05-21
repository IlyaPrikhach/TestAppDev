using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TestAppDev.Data;
using TestAppDev.Interfaces;

namespace TestAppDev.Controllers;

/// <summary>
/// Represents tree node API
/// </summary>
[SwaggerTag("user.tree.node")]
[ApiController]
public class NodesController : ControllerBase
{
    private readonly INodeService _nodeService;

    public NodesController(
        INodeService nodeService)
    {
        _nodeService = nodeService;
    }

    /// <summary>
    /// Create a new node in your tree. You must to specify a parent node ID that belongs to your tree.
    /// A new node name must be unique across all siblings.
    /// </summary>
    [Route("/api.user.tree.node.create")]
    [HttpPost]
    public async Task<ActionResult> CreateNode([FromQuery] string treeName, [FromQuery] Guid parentNodeId, [FromQuery] string nodeName )
    {
        await _nodeService.CreateNodeAsync(treeName, parentNodeId, nodeName);
        return Ok();
    }

    /// <summary>
    /// Delete an existing node in your tree. You must specify a node ID that belongs your tree.
    /// </summary>
    [Route("/api.user.tree.node.delete")]
    [HttpDelete]
    public async Task<ActionResult> DeleteNode([FromQuery] string treeName, [FromQuery] Guid nodeId)
    {
        await _nodeService.DeleteNodeAsync(treeName, nodeId);
        return Ok();
    }

    /// <summary>
    /// Rename an existing node in your tree. You must specify a node ID that belongs your tree.
    /// A new name of the node must be unique across all siblings.
    /// </summary>
    [Route("/api.user.tree.node.rename")]
    [HttpPut]
    public async Task<ActionResult> RenameNode([FromQuery] string treeName, [FromQuery] Guid nodeId, [FromQuery] string noewNodeName)
    {
        await _nodeService.RenameNodeAsync(treeName, nodeId, noewNodeName);
        return Ok();
    }
}
