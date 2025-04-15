using MediatR;
using Microsoft.AspNetCore.Mvc;
using PhysicalPersonsDirectory.Application.Commands;
using PhysicalPersonsDirectory.Application.Queries;

namespace PhysicalPersonsDirectory.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhysicalPersonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PhysicalPersonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePhysicalPersonCommand command)
    {
        var id = await _mediator.Send(command);
        var person = await _mediator.Send(new GetPhysicalPersonByIdQuery { Id = id });
        return CreatedAtAction(nameof(GetById), new { id }, person);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var person = await _mediator.Send(new GetPhysicalPersonByIdQuery { Id = id });
        return person != null ? Ok(person) : NotFound();
    }
}