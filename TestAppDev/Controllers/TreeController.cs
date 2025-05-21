using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TestAppDev.Interfaces;
using TestAppDev.Models;

namespace TestAppDev.Controllers;

/// <summary>
/// Represents journal API
/// </summary>
[SwaggerTag("user.tree")]
[ApiController]
public class TreeController: ControllerBase
{
    private readonly INodeService _nodeService;

    public TreeController(INodeService nodeService)
    {
        _nodeService = nodeService;
    }

    /// <summary>
    /// Returns your entire tree. If your tree doesn't exist it will be created automatically.
    /// </summary>
    [Route("/api.user.tree.get")]
    [HttpPost]
    public async Task<ActionResult<TreeResponse>> GetTree([FromQuery] string treeName)
    {
        if (string.IsNullOrWhiteSpace(treeName))
            return BadRequest("Tree name is required");

        var nodes = await _nodeService.GetOrCreateTreeNodesAsync(treeName);
        var rootNode = nodes.FirstOrDefault(n => n.ParentId == null);

        if (rootNode == null)
            throw new Exception("Failed to create tree");

        var response = new TreeResponse
        {
            Id = rootNode.TreeId,
            Name = rootNode.Name,
            Children = rootNode.Children
        };

        return Ok(response);
    }
}
