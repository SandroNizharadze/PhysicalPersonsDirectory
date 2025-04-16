using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhysicalPersonsDirectory.Domain;

namespace PhysicalPersonsDirectory.Infrastructure.Persistence;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public DbSet<PhysicalPerson> PhysicalPersons { get; set; }
    public DbSet<City> Cities { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .LogTo(Console.WriteLine, LogLevel.Debug) // Force SQL logging to console
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PhysicalPerson>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<PhysicalPerson>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<PhysicalPerson>()
            .HasMany(p => p.PhoneNumbers)
            .WithOne()
            .HasForeignKey(p => p.PhysicalPersonId);

        modelBuilder.Entity<PhysicalPerson>()
            .HasMany(p => p.RelatedPersons)
            .WithOne()
            .HasForeignKey(r => r.PhysicalPersonId);

        modelBuilder.Entity<RelatedPerson>()
            .HasKey(r => new { r.PhysicalPersonId, r.RelatedPhysicalPersonId });

        modelBuilder.Entity<RelatedPerson>()
            .HasOne<PhysicalPerson>()
            .WithMany()
            .HasForeignKey(r => r.RelatedPhysicalPersonId);
    }

    public async Task SaveChangesAsync()
    {
        await base.SaveChangesAsync();
    }
}