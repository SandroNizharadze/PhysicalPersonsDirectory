using MediatR;
using PhysicalPersonsDirectory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace PhysicalPersonsDirectory.Application.Queries;

public class GetRelatedPersonsReportQuery : IRequest<List<RelatedPersonsReportDto>>
{
}

public class RelatedPersonsReportDto
{
    public int PersonId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required Dictionary<string, int> RelationCounts { get; set; }
}

public class GetRelatedPersonsReportQueryHandler : IRequestHandler<GetRelatedPersonsReportQuery, List<RelatedPersonsReportDto>>
{
    private readonly ApplicationDbContext _context;

    public GetRelatedPersonsReportQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RelatedPersonsReportDto>> Handle(GetRelatedPersonsReportQuery request, CancellationToken cancellationToken)
    {
        var personsWithRelations = await _context.PhysicalPersons
            .AsNoTracking()
            .Include(p => p.RelatedPersons)
            .Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                RelatedPersons = p.RelatedPersons
                    .GroupBy(r => r.RelationType)
                    .Select(g => new { RelationType = g.Key.ToString(), Count = g.Count() })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var report = personsWithRelations.Select(p => new RelatedPersonsReportDto
        {
            PersonId = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            RelationCounts = p.RelatedPersons.ToDictionary(
                r => r.RelationType,
                r => r.Count)
        }).ToList();

        return report;
    }
}