using MediatR;
using PhysicalPersonsDirectory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace PhysicalPersonsDirectory.Application.Commands;

public class UploadPhysicalPersonImageCommand : IRequest<string>
{
    public int Id { get; set; }
    public required IFormFile File { get; set; }
}

public class UploadPhysicalPersonImageCommandHandler : IRequestHandler<UploadPhysicalPersonImageCommand, string>
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public UploadPhysicalPersonImageCommandHandler(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<string> Handle(UploadPhysicalPersonImageCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.PhysicalPersons
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (person == null)
        {
            throw new Exception($"Person with ID {request.Id} not found.");
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileExtension = Path.GetExtension(request.File.FileName).ToLower();
        if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(fileExtension))
        {
            throw new Exception("InvalidImageFormat");
        }

        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        person.ImagePath = $"/Uploads/{fileName}";
        await _context.SaveChangesAsync(cancellationToken);

        return person.ImagePath;
    }
}