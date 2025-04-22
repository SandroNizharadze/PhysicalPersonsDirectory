using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging; // Add this for logging
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
    private readonly IStringLocalizer<PhysicalPersonsController> _localizer;
    private readonly ILogger<PhysicalPersonsController> _logger; // Add logger

    public PhysicalPersonsController(
        ISender sender, 
        IStringLocalizer<PhysicalPersonsController> localizer, 
        ILogger<PhysicalPersonsController> logger) // Inject logger
    {
        _sender = sender;
        _localizer = localizer;
        _logger = logger;
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

    [HttpGet("report")]
    public async Task<IActionResult> GetRelatedPersonsReport(CancellationToken cancellationToken)
    {
        var query = new GetRelatedPersonsReportQuery();
        var report = await _sender.Send(query, cancellationToken);
        return Ok(report);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePhysicalPersonCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting to create a new PhysicalPerson with FirstName: {FirstName}", command.FirstName);
            var personId = await _sender.Send(command, cancellationToken);
            _logger.LogInformation("Successfully created PhysicalPerson with ID: {PersonId}", personId);
            return CreatedAtAction(nameof(Get), new { id = personId }, new { Id = personId });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("DomainException caught in Create action: {Message}", ex.Message);
            return BadRequest(new { Error = _localizer["InvalidRequest"].Value, Details = _localizer[ex.Message].Value });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("ArgumentException caught in Create action: {Message}", ex.Message);
            return BadRequest(new { Error = _localizer["InvalidRequest"].Value, Details = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Create action");
            throw;
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
            return BadRequest(new { Error = "IDMismatch", Details = _localizer["IDMismatch"].Value });
        }

        try
        {
            await _sender.Send(command, cancellationToken);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = _localizer["InvalidRequest"].Value, Details = ex.Message });
        }
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
            return BadRequest(new { Error = "ImageRequired", Details = _localizer["ImageRequired"].Value });
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
            return BadRequest(new { Error = "ImageUploadFailed", Details = _localizer["ImageUploadFailed", ex.Message].Value });
        }
    }

    [HttpPost("{id}/related")]
    public async Task<IActionResult> AddRelatedPerson(int id, [FromBody] AddRelatedPersonRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Error = _localizer["ValidationFailed"].Value, Details = ModelState.Values.SelectMany(v => v.Errors).Select(e => _localizer[e.ErrorMessage].Value) });
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
            return BadRequest(new { Error = _localizer["InvalidRequest"].Value, Details = ex.Message });
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
            return BadRequest(new { Error = _localizer["InvalidRequest"].Value, Details = ex.Message });
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