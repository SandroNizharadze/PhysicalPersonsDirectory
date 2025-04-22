using AutoMapper;
using MediatR;
using PhysicalPersonsDirectory.Application.DTOs;
using PhysicalPersonsDirectory.Domain;
using PhysicalPersonsDirectory.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace PhysicalPersonsDirectory.Application.Queries;

public class GetPhysicalPersonByIdQuery : IRequest<PhysicalPersonDto>
{
    public int Id { get; set; }
}

public class GetPhysicalPersonByIdQueryHandler : IRequestHandler<GetPhysicalPersonByIdQuery, PhysicalPersonDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPhysicalPersonByIdQueryHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PhysicalPersonDto> Handle(GetPhysicalPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await _context.PhysicalPersons
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.PhoneNumbers)
            .Include(p => p.RelatedPersons)
            .Include(p => p.City)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (person == null)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        return _mapper.Map<PhysicalPersonDto>(person);
    }
}