using Microsoft.EntityFrameworkCore;
using PhysicalPersonsDirectory.Domain;
using PhysicalPersonsDirectory.Domain.Interfaces;
using PhysicalPersonsDirectory.Infrastructure.Persistence;

namespace PhysicalPersonsDirectory.Infrastructure.Repositories;

public class PhysicalPersonRepository : IPhysicalPersonRepository
{
    private readonly ApplicationDbContext _context;

    public PhysicalPersonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PhysicalPerson> GetByIdAsync(int id)
    {
        var person = await _context.PhysicalPersons
            .Include(p => p.PhoneNumbers)
            .Include(p => p.RelatedPersons)
            .Include(p => p.City)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (person == null)
            throw new KeyNotFoundException($"PhysicalPerson with ID {id} was not found.");

        return person;
    }

    public async Task AddAsync(PhysicalPerson person)
    {
        await _context.PhysicalPersons.AddAsync(person);
        _context.Entry(person).State = EntityState.Added; // Ensure entity is tracked
    }

    


    public async Task<List<PhysicalPerson>> GetAllAsync()
    {
        return await _context.PhysicalPersons
            .Include(p => p.PhoneNumbers)
            .Include(p => p.RelatedPersons)
            .Include(p => p.City)
            .ToListAsync();
    }

    public Task UpdateAsync(PhysicalPerson person)
    {
        _context.PhysicalPersons.Update(person);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var person = await GetByIdAsync(id);
        if (person != null)
            _context.PhysicalPersons.Remove(person);
    }

    public async Task<List<PhysicalPerson>> SearchAsync(string searchTerm)
    {
        return await _context.PhysicalPersons
            .Where(p => p.FirstName.Contains(searchTerm) || 
                        p.LastName.Contains(searchTerm) || 
                        p.PersonalNumber.Contains(searchTerm))
            .Include(p => p.City)
            .ToListAsync();
    }

    public async Task<List<PhysicalPerson>> DetailedSearchAsync(PhysicalPersonSearchCriteria criteria)
    {
        var query = _context.PhysicalPersons.AsQueryable();

        if (!string.IsNullOrEmpty(criteria.FirstName))
            query = query.Where(p => p.FirstName.Contains(criteria.FirstName));

        if (!string.IsNullOrEmpty(criteria.LastName))
            query = query.Where(p => p.LastName.Contains(criteria.LastName));

        if (criteria.Gender.HasValue)
            query = query.Where(p => p.Gender == criteria.Gender.Value);

        if (!string.IsNullOrEmpty(criteria.PersonalNumber))
            query = query.Where(p => p.PersonalNumber == criteria.PersonalNumber);

        if (criteria.DateOfBirth.HasValue)
            query = query.Where(p => p.DateOfBirth == criteria.DateOfBirth.Value);

        if (criteria.CityId.HasValue)
            query = query.Where(p => p.CityId == criteria.CityId.Value);

        return await query
            .Include(p => p.City)
            .Include(p => p.PhoneNumbers)
            .ToListAsync();
    }

    public async Task<List<RelatedPersonReport>> GetRelatedPersonsReportAsync()
    {
        return await _context.PhysicalPersons
            .Select(p => new RelatedPersonReport
            {
                PhysicalPersonId = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                ColleagueCount = p.RelatedPersons.Count(r => r.RelationType == RelationType.Colleague),
                AcquaintanceCount = p.RelatedPersons.Count(r => r.RelationType == RelationType.Acquaintance),
                RelativeCount = p.RelatedPersons.Count(r => r.RelationType == RelationType.Relative),
                OtherCount = p.RelatedPersons.Count(r => r.RelationType == RelationType.Other)
            })
            .ToListAsync();
    }
}