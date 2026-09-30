using System;
using Microsoft.AspNetCore.Mvc;

namespace Indiana.AI.Api.Controllers;

/// <summary>
/// Lightweight status / liveness endpoint for the integration gateway.
/// </summary>
[ApiController]
[Route("api")]
[Produces("application/json")]
public class StatusController : ControllerBase
{
    /// <summary>Returns a simple status heartbeat for the service.</summary>
    /// <response code="200">The service is up.</response>
    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetStatus() => Ok(new
    {
        service = "Indiana.AI.Integration",
        status = "UP",
        component = "AI Integration Gateway",
        utc = DateTime.UtcNow
    });
}
