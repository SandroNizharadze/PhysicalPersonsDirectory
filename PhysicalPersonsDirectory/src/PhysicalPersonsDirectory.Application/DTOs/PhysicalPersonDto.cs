using PhysicalPersonsDirectory.Domain;

namespace PhysicalPersonsDirectory.Application.DTOs;

public class PhysicalPersonDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public Gender Gender { get; set; }
    public required string PersonalNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int CityId { get; set; }
    public required string CityName { get; set; }
    public required List<PhoneNumberDto> PhoneNumbers { get; set; }
    public required string ImagePath { get; set; }
    public required List<RelatedPersonDto> RelatedPersons { get; set; }
}

public class PhoneNumberDto
{
    public PhoneType Type { get; set; }
    public required string Number { get; set; }
}

public class RelatedPersonDto
{
    public int RelatedPhysicalPersonId { get; set; }
    public RelationType RelationType { get; set; }
}