using MediatR;
using Microsoft.AspNetCore.Mvc;
using PhysicalPersonsDirectory.Application.Commands;
using PhysicalPersonsDirectory.Application.Queries;
using PhysicalPersonsDirectory.Domain;

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
        var personId = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = personId }, new { Id = personId });
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

        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeletePhysicalPersonCommand { Id = id };
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { Error = "ImageRequired", Details = "No file was uploaded." });
        }

        var command = new UploadPhysicalPersonImageCommand
        {
            Id = id,
            File = file
        };

        try
        {
            var imagePath = await _sender.Send(command, cancellationToken);
            return Ok(new { ImagePath = imagePath });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "ImageUploadFailed", Details = ex.Message });
        }
    }

    [HttpPost("{id}/related")]
    public async Task<IActionResult> AddRelatedPerson(int id, [FromBody] AddRelatedPersonRequest request, CancellationToken cancellationToken)
    {
        var command = new AddRelatedPersonCommand
        {
            Id = id,
            RelatedPhysicalPersonId = request.RelatedPhysicalPersonId,
            RelationType = request.RelationType
        };

        await _sender.Send(command, cancellationToken);
        return Ok();
    }

    [HttpDelete("{id}/related/{relatedId}")]
    public async Task<IActionResult> RemoveRelatedPerson(int id, int relatedId, CancellationToken cancellationToken)
    {
        var command = new RemoveRelatedPersonCommand
        {
            Id = id,
            RelatedPhysicalPersonId = relatedId
        };

        await _sender.Send(command, cancellationToken);
        return NoContent();
    }
}

public class AddRelatedPersonRequest
{
    public int RelatedPhysicalPersonId { get; set; }
    public RelationType RelationType { get; set; }
}