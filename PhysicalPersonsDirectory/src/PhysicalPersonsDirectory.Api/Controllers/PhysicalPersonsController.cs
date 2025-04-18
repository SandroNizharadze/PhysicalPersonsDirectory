using MediatR;
using Microsoft.AspNetCore.Mvc;
using PhysicalPersonsDirectory.Application.Commands;
using PhysicalPersonsDirectory.Application.Queries;

namespace PhysicalPersonsDirectory.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhysicalPersonsController : ControllerBase
{
    private readonly ISender _sender;

    public PhysicalPersonsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePhysicalPersonCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var personId = await _sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = personId }, new { Id = personId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while creating the person.", Details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var query = new GetPhysicalPersonByIdQuery { Id = id };
        var result = await _sender.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePhysicalPersonCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new { Error = "ID in URL does not match ID in body." });
        }

        try
        {
            await _sender.Send(command, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while updating the person.", Details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeletePhysicalPersonCommand { Id = id };
            await _sender.Send(command, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while deleting the person.", Details = ex.Message });
        }
    }
}