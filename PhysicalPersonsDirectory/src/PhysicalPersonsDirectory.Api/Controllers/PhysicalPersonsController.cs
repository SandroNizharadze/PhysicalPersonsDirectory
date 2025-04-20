using MediatR;
using Microsoft.AspNetCore.Mvc;
using PhysicalPersonsDirectory.Application.Commands;
using PhysicalPersonsDirectory.Application.Queries;
using PhysicalPersonsDirectory.Domain;
using System.ComponentModel.DataAnnotations;

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

    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] string? quickSearch,
        [FromQuery] string? firstName,
        [FromQuery] string? lastName,
        [FromQuery] string? personalNumber,
        [FromQuery] Gender? gender,
        [FromQuery] DateTime? dateOfBirth,
        [FromQuery] int? cityId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPhysicalPersonsQuery
        {
            QuickSearch = quickSearch,
            FirstName = firstName,
            LastName = lastName,
            PersonalNumber = personalNumber,
            Gender = gender,
            DateOfBirth = dateOfBirth,
            CityId = cityId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
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
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Error = "ValidationFailed", Details = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        var command = new AddRelatedPersonCommand
        {
            Id = id,
            RelatedPhysicalPersonId = request.RelatedPhysicalPersonId,
            RelationType = request.RelationType
        };

        try
        {
            await _sender.Send(command, cancellationToken);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = "InvalidRequest", Details = ex.Message });
        }
    }

    [HttpDelete("{id}/related/{relatedId}")]
    public async Task<IActionResult> RemoveRelatedPerson(int id, int relatedId, CancellationToken cancellationToken)
    {
        var command = new RemoveRelatedPersonCommand
        {
            Id = id,
            RelatedPhysicalPersonId = relatedId
        };

        try
        {
            await _sender.Send(command, cancellationToken);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = "InvalidRequest", Details = ex.Message });
        }
    }
}

public class AddRelatedPersonRequest
{
    [Required(ErrorMessage = "RelatedPhysicalPersonIdRequired")]
    [Range(1, int.MaxValue, ErrorMessage = "RelatedPhysicalPersonIdMustBePositive")]
    public int RelatedPhysicalPersonId { get; set; }

    [Required(ErrorMessage = "RelationTypeRequired")]
    public RelationType RelationType { get; set; }
}