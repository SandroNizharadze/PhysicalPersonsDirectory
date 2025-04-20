using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using PhysicalPersonsDirectory.Application.DTOs;
using PhysicalPersonsDirectory.Domain;
using PhysicalPersonsDirectory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace PhysicalPersonsDirectory.Application.Queries;

public class GetPhysicalPersonsQuery : IRequest<PagedResult<PhysicalPersonDto>>
{
    public string? QuickSearch { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PersonalNumber { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? CityId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PagedResult<T>
{
    public required List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public class GetPhysicalPersonsQueryHandler : IRequestHandler<GetPhysicalPersonsQuery, PagedResult<PhysicalPersonDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPhysicalPersonsQueryHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<PhysicalPersonDto>> Handle(GetPhysicalPersonsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PhysicalPersons
            .AsNoTracking()
            .Include(p => p.PhoneNumbers)
            .Include(p => p.RelatedPersons)
            .Include(p => p.City)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.QuickSearch))
        {
            var searchTerm = $"%{request.QuickSearch}%";
            query = query.Where(p =>
                EF.Functions.Like(p.FirstName, searchTerm) ||
                EF.Functions.Like(p.LastName, searchTerm) ||
                EF.Functions.Like(p.PersonalNumber, searchTerm));
        }

        // Search
        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            query = query.Where(p => p.FirstName.Contains(request.FirstName));
        }

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            query = query.Where(p => p.LastName.Contains(request.LastName));
        }

        if (!string.IsNullOrWhiteSpace(request.PersonalNumber))
        {
            query = query.Where(p => p.PersonalNumber == request.PersonalNumber);
        }

        if (request.Gender.HasValue)
        {
            query = query.Where(p => p.Gender == request.Gender.Value);
        }

        if (request.DateOfBirth.HasValue)
        {
            query = query.Where(p => p.DateOfBirth.Date == request.DateOfBirth.Value.Date);
        }

        if (request.CityId.HasValue)
        {
            query = query.Where(p => p.CityId == request.CityId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<PhysicalPersonDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<PhysicalPersonDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}