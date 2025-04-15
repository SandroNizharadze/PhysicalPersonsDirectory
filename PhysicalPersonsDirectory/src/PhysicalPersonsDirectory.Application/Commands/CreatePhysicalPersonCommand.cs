using MediatR;
using PhysicalPersonsDirectory.Application.DTOs;
using PhysicalPersonsDirectory.Domain;
using PhysicalPersonsDirectory.Domain.Interfaces;
using PhysicalPersonsDirectory.Infrastructure.Persistence;

namespace PhysicalPersonsDirectory.Application.Commands;

public class CreatePhysicalPersonCommand : IRequest<int>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public Gender Gender { get; set; }
    public required string PersonalNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int CityId { get; set; }
    public required List<PhoneNumberDto> PhoneNumbers { get; set; }
}

public class CreatePhysicalPersonCommandHandler : IRequestHandler<CreatePhysicalPersonCommand, int>
{
    private readonly IPhysicalPersonRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePhysicalPersonCommandHandler(IPhysicalPersonRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreatePhysicalPersonCommand request, CancellationToken cancellationToken)
    {
        var person = new PhysicalPerson(
            request.FirstName,
            request.LastName,
            request.Gender,
            request.PersonalNumber,
            request.DateOfBirth,
            request.CityId);

        foreach (var phone in request.PhoneNumbers)
        {
            person.AddPhoneNumber(new PhoneNumber(phone.Type, phone.Number));
        }

        await _repository.AddAsync(person);
        await _unitOfWork.SaveChangesAsync();

        return person.Id;
    }
}