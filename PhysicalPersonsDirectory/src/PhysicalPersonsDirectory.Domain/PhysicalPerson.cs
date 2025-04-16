namespace PhysicalPersonsDirectory.Domain;

public class PhysicalPerson
{
    public int Id { get; set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Gender Gender { get; private set; }
    public string PersonalNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public int CityId { get; private set; }
    public City? City { get; private set; }
    public string? ImagePath { get; private set; }
    public List<PhoneNumber> PhoneNumbers { get; private set; } = new();
    public List<RelatedPerson> RelatedPersons { get; private set; } = new();

    public PhysicalPerson(string firstName, string lastName, Gender gender, string personalNumber, DateTime dateOfBirth, int cityId)
    {
        FirstName = firstName;
        LastName = lastName;
        Gender = gender;
        PersonalNumber = personalNumber;
        DateOfBirth = dateOfBirth.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(dateOfBirth, DateTimeKind.Utc)
            : dateOfBirth.ToUniversalTime();
        CityId = cityId;
    }

    private string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2 || name.Length > 50)
            throw new DomainException("Name must be between 2 and 50 characters.");
        
        if (!IsGeorgianOrLatin(name))
            throw new DomainException("Name must contain only Georgian or Latin letters.");

        return name;
    }

    private string ValidatePersonalNumber(string number)
    {
        if (number.Length != 11 || !number.All(char.IsDigit))
            throw new DomainException("Personal number must be 11 digits.");

        return number;
    }

    private DateTime ValidateDateOfBirth(DateTime dob)
    {
        if (dob > DateTime.Now.AddYears(-18))
            throw new DomainException("Person must be at least 18 years old.");

        return dob;
    }

    private bool IsGeorgianOrLatin(string text)
    {
        bool isGeorgian = text.All(c => c >= 0x10D0 && c <= 0x10FF);
        bool isLatin = text.All(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'));
        return isGeorgian || isLatin;
    }

    public void UpdateImagePath(string path) => ImagePath = path;

    public void AddPhoneNumber(PhoneNumber phoneNumber) => PhoneNumbers.Add(phoneNumber);

    public void AddRelatedPerson(RelatedPerson relatedPerson) => RelatedPersons.Add(relatedPerson);

    public void RemoveRelatedPerson(int relatedPersonId) => 
        RelatedPersons.RemoveAll(r => r.RelatedPhysicalPersonId == relatedPersonId);
}

public enum Gender
{
    Female,
    Male
}

public class PhoneNumber
{
    public int Id { get; private set; }
    public int PhysicalPersonId { get; private set; }
    public PhoneType Type { get; private set; }
    public string Number { get; private set; }

    public PhoneNumber(PhoneType type, string number)
    {
        Type = type;
        Number = ValidateNumber(number);
    }

    private string ValidateNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Length < 4 || number.Length > 50)
            throw new DomainException("Phone number must be between 4 and 50 characters.");
        return number;
    }
}

public enum PhoneType
{
    Mobile,
    Office,
    Home
}

public class RelatedPerson
{
    public int PhysicalPersonId { get; private set; }
    public int RelatedPhysicalPersonId { get; private set; }
    public RelationType RelationType { get; private set; }

    public RelatedPerson(int relatedPhysicalPersonId, RelationType relationType)
    {
        RelatedPhysicalPersonId = relatedPhysicalPersonId;
        RelationType = relationType;
    }
}

public enum RelationType
{
    Colleague,
    Acquaintance,
    Relative,
    Other
}

public class City
{
    public int Id { get; private set; }
    public string? Name { get; private set; }
}

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}