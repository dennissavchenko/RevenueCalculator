using Microsoft.EntityFrameworkCore;
using Revenue.Entities;

namespace Revenue.Context;

public class SystemContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<IndividualClient> IndividualClients { get; set; }
    public DbSet<CompanyClient> CompanyClients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(x => x.IdClient);
            entity.ToTable("Client");
            entity
                .Property(x => x.IdClient)
                .ValueGeneratedOnAdd()
                .IsRequired();
            entity
                .HasDiscriminator<string>("ClientType")
                .HasValue<IndividualClient>("Individual")
                .HasValue<CompanyClient>("Company");
            entity
                .Property(x => x.Address)
                .HasMaxLength(100)
                .IsRequired();
            entity
                .Property(x => x.Email)
                .HasMaxLength(100)
                .IsRequired();
            entity
                .Property(x => x.PhoneNumber)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<IndividualClient>(entity =>
        {
            entity
                .Property(x => x.PESEL)
                .HasMaxLength(11)
                .IsRequired()
                .ValueGeneratedNever();
            entity
                .Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();
            entity
                .Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();
            entity
                .Property(x => x.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();
            entity
                .HasIndex(x => x.PESEL)
                .IsUnique();
        });

        modelBuilder.Entity<CompanyClient>(entity =>
        {
            entity
                .Property(x => x.KRS)
                .HasMaxLength(14)
                .IsRequired()
                .ValueGeneratedNever();
            entity
                .Property(x => x.CompanyName)
                .HasMaxLength(100)
                .IsRequired();
            entity
                .HasIndex(x => x.KRS)
                .IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }

    public SystemContext(DbContextOptions<SystemContext> options) : base(options)
    {
    }
}
