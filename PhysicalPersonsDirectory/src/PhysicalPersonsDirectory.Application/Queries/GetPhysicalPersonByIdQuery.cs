using AutoMapper;
using MediatR;
using PhysicalPersonsDirectory.Application.DTOs;
using PhysicalPersonsDirectory.Domain.Interfaces;

namespace PhysicalPersonsDirectory.Application.Queries;

public class GetPhysicalPersonByIdQuery : IRequest<PhysicalPersonDto>
{
    public int Id { get; set; }
}

public class GetPhysicalPersonByIdQueryHandler : IRequestHandler<GetPhysicalPersonByIdQuery, PhysicalPersonDto>
{
    private readonly IPhysicalPersonRepository _repository;
    private readonly IMapper _mapper;

    public GetPhysicalPersonByIdQueryHandler(IPhysicalPersonRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PhysicalPersonDto> Handle(GetPhysicalPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await _repository.GetByIdAsync(request.Id);
        if (person == null)
        {
            return null;
        }
        return _mapper.Map<PhysicalPersonDto>(person);
    }
}