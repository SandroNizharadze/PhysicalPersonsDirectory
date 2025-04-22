using Microsoft.EntityFrameworkCore;
using PhysicalPersonsDirectory.Domain;

namespace PhysicalPersonsDirectory.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<PhysicalPerson> PhysicalPersons { get; set; }
    public DbSet<PhoneNumber> PhoneNumbers { get; set; }
    public DbSet<RelatedPerson> RelatedPersons { get; set; }
    public DbSet<City> Cities { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PhysicalPerson>()
            .HasMany(p => p.PhoneNumbers)
            .WithOne()
            .HasForeignKey(p => p.PhysicalPersonId);

        modelBuilder.Entity<PhysicalPerson>()
            .HasMany(p => p.RelatedPersons)
            .WithOne()
            .HasForeignKey(r => r.PhysicalPersonId);

        modelBuilder.Entity<PhysicalPerson>()
            .HasOne(p => p.City)
            .WithMany()
            .HasForeignKey(p => p.CityId);

        modelBuilder.Entity<RelatedPerson>()
            .HasKey(r => new { r.PhysicalPersonId, r.RelatedPhysicalPersonId });
    }
}