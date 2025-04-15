namespace PhysicalPersonsDirectory.Domain.Interfaces;

public interface IPhysicalPersonRepository
{
    Task<PhysicalPerson> GetByIdAsync(int id);
    Task<List<PhysicalPerson>> GetAllAsync();
    Task AddAsync(PhysicalPerson person);
    Task UpdateAsync(PhysicalPerson person);
    Task DeleteAsync(int id);
    Task<List<PhysicalPerson>> SearchAsync(string searchTerm);
    Task<List<PhysicalPerson>> DetailedSearchAsync(PhysicalPersonSearchCriteria criteria);
    Task<List<RelatedPersonReport>> GetRelatedPersonsReportAsync();
}

public class PhysicalPersonSearchCriteria
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Gender? Gender { get; set; }
    public string? PersonalNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? CityId { get; set; }
}

public class RelatedPersonReport
{
    public int PhysicalPersonId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int ColleagueCount { get; set; }
    public int AcquaintanceCount { get; set; }
    public int RelativeCount { get; set; }
    public int OtherCount { get; set; }
}