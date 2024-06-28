using Microsoft.EntityFrameworkCore;
using Revenue.Entities;

namespace Revenue.Context;

public class SystemContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Software> Software { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<ContractPayment> ContractPayments { get; set; }
    public DbSet<IndividualClient> IndividualClients { get; set; }
    public DbSet<CompanyClient> CompanyClients { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }
    public DbSet<SubscriptionClient> SubscriptionClients { get; set; }
    public DbSet<Employee> Employees { get; set; }


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
                .HasDiscriminator(x => x.ClientTypeId)
                .HasValue<IndividualClient>(1)
                .HasValue<CompanyClient>(2);
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
            entity
                .HasOne(x => x.ClientType)
                .WithMany(x => x.Clients)
                .HasForeignKey(x => x.ClientTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<Software>(entity =>
        {
            entity.HasKey(x => x.IdSoftware);
            entity.ToTable("Software");
            entity
                .Property(x => x.IdSoftware)
                .ValueGeneratedOnAdd()
                .IsRequired();
            entity
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
            entity
                .Property(x => x.Description)
                .HasMaxLength(300)
                .IsRequired();
            entity
                .Property(x => x.CurrentVersion)
                .HasMaxLength(100)
                .IsRequired();
            entity
                .Property(x => x.SubscriptionCost)
                .IsRequired();
            entity
                .Property(x => x.UpfrontCost)
                .IsRequired();
            entity
                .HasOne(x => x.SoftwareCategory)
                .WithMany(x => x.Softwares)
                .HasForeignKey(x => x.IdSoftwareCategory);
            entity
                .HasMany(x => x.Discounts)
                .WithMany(x => x.Softwares)
                .UsingEntity(
                    "SoftwareDiscount",
                    r => r.HasOne(typeof(Discount)).WithMany().HasForeignKey("IdDiscount"),
                    l => l.HasOne(typeof(Software)).WithMany().HasForeignKey("IdSoftware"),
                    j => j.HasKey("IdDiscount", "IdSoftware")
                );
        });
        
        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(x => x.IdPaymentStatus);
            entity.ToTable("PaymentStatus");
            entity
                .Property(x => x.IdPaymentStatus)
                .ValueGeneratedOnAdd()
                .IsRequired();
            entity
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
        });
        
        modelBuilder.Entity<SoftwareCategory>(entity =>
        {
            entity.HasKey(x => x.IdSoftwareCategory);
            entity.ToTable("SoftwareCategory");
            entity
                .Property(x => x.IdSoftwareCategory)
                .ValueGeneratedOnAdd()
                .IsRequired();
            entity
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
        });
        
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(x => x.IdContract);
            entity.ToTable("Contract");
            entity
                .Property(x => x.IdContract)
                .ValueGeneratedOnAdd()
                .IsRequired();
            entity
                .Property(x => x.StartDate)
                .IsRequired();
            entity
                .Property(x => x.EndDate)
                .IsRequired();
            entity
                .Property(x => x.Price)
                .IsRequired();
            entity
                .Property(x => x.AdditionalYearsOfSupport)
                .IsRequired();
            entity
                .Property(x => x.IsSigned)
                .IsRequired();
            entity
                .Property(x => x.IsCancelled)
                .IsRequired();
            entity
                .Property(x => x.SoftwareVersion)
                .HasMaxLength(100)
                .IsRequired();
            entity
                .HasOne(x => x.Software)
                .WithMany(x => x.Contracts)
                .HasForeignKey(x => x.IdSoftware);
            entity
                .HasOne(x => x.Client)
                .WithMany(x => x.Contracts)
                .HasForeignKey(x => x.IdClient);
        });
        
        modelBuilder.Entity<ContractPayment>(entity =>
        {
            entity.HasKey(x => x.IdContractPayment);
            entity.ToTable("ContractPayment");
            entity
                .Property(x => x.IdContractPayment)
                .ValueGeneratedOnAdd()
                .IsRequired();
            entity
                .Property(x => x.Date)
                .IsRequired();
            entity
                .Property(x => x.Amount)
                .IsRequired();
            entity
                .HasOne(x => x.Contract)
                .WithMany(x => x.ContractPayments)
                .HasForeignKey(x => x.IdContract);
            entity
                .HasOne(x => x.PaymentStatus)
                .WithMany(x => x.ContractPayments)
                .HasForeignKey(x => x.IdPaymentStatus);
        });
        
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(x => x.IdEmployee);
            entity.ToTable("Employee");
            entity
                .Property(x => x.IdEmployee)
                .ValueGeneratedOnAdd()
                .IsRequired();
            entity
                .Property(x => x.Username)
                .HasMaxLength(100)
                .IsRequired();
            entity
                .Property(x => x.Password)
                .HasMaxLength(100)
                .IsRequired();
            entity
                .HasOne(x => x.EmployeeRole)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.IdEmployeeRole);
        });
        
        modelBuilder.Entity<EmployeeRole>(entity =>
        {
            entity.HasKey(x => x.IdEmployeeRole);
            entity.ToTable("EmployeeRole");
            entity
                .Property(x => x.IdEmployeeRole)
                .ValueGeneratedOnAdd()
                .IsRequired();
            entity
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
        });
        
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(x => x.IdSubscription);
            entity.ToTable("Subscription");
            entity
                .Property(x => x.IdSubscription)
                .ValueGeneratedOnAdd()
                .IsRequired();
            entity
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
            entity
                .Property(x => x.Price)
                .IsRequired();
            entity
                .Property(x => x.RenewalPeriodDays)
                .IsRequired();
            entity
                .HasOne(x => x.Software)
                .WithMany(x => x.Subscriptions)
                .HasForeignKey(x => x.IdSoftware);
        });
        
        modelBuilder.Entity<SubscriptionPayment>(entity =>
        {
            entity.HasKey(x => x.IdSubscriptionPayment);
            entity.ToTable("SubscriptionPayment");
            entity
                .Property(x => x.IdSubscriptionPayment)
                .ValueGeneratedOnAdd()
                .IsRequired();
            entity
                .Property(x => x.Date)
                .IsRequired();
            entity
                .Property(x => x.Amount)
                .IsRequired();
            entity
                .HasOne(x => x.SubscriptionClient)
                .WithMany(x => x.SubscriptionPayments)
                .HasForeignKey(x => x.IdSubscriptionClient);
        });
        
        modelBuilder.Entity<SubscriptionClient>(entity =>
        {
            entity.HasKey(x => x.IdSubscriptionClient);
            entity.ToTable("SubscriptionClient");
            entity
                .Property(x => x.IdSubscriptionClient)
                .ValueGeneratedOnAdd()
                .IsRequired();
            entity
                .Property(x => x.IsCancelled)
                .IsRequired();
            entity
                .Property(x => x.Price)
                .IsRequired();
            entity
                .Property(x => x.NextPaymentDate)
                .IsRequired();
            entity
                .HasOne(x => x.Client)
                .WithMany(x => x.SubscriptionClients)
                .HasForeignKey(x => x.IdClient);
            entity
                .HasOne(x => x.Subscription)
                .WithMany(x => x.SubscriptionClients)
                .HasForeignKey(x => x.IdSubscription);
        });
        
        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(x => x.IdDiscount);
            entity.ToTable("Discount");
            entity
                .Property(x => x.IdDiscount)
                .ValueGeneratedOnAdd()
                .IsRequired();
            entity
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
            entity
                .Property(x => x.Percentage)
                .IsRequired();
            entity
                .Property(x => x.DateFrom)
                .IsRequired();
            entity
                .Property(x => x.DateTo)
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
        
        modelBuilder.Entity<ClientType>(entity =>
        {
            entity.HasKey(x => x.ClientTypeId);
            entity.ToTable("ClientType");
            entity
                .Property(x => x.ClientTypeId)
                .IsRequired()
                .ValueGeneratedOnAdd();
            entity
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }

    public SystemContext(DbContextOptions<SystemContext> options) : base(options)
    {
    }
}
