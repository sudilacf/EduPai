using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;
using WebApplication1.Identity;

namespace WebApplication1.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    
    public DbSet<School> Schools { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentSchoolAccount> StudentSchoolAccounts { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<BalanceEntry> BalanceEntries { get; set; }
    public DbSet<PaymentAllocation> PaymentAllocations { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<SchoolAdminInvitation> SchoolAdminInvitations { get; set; }
    public DbSet<SchoolPaymentConfiguration> SchoolPaymentConfigurations { get; set; }
    public DbSet<SchoolMember> SchoolMembers { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // User <-> Student: one-to-one
        builder.Entity<Student>()
            .HasIndex(x => x.UserId)
            .IsUnique();

        builder.Entity<Student>()
            .HasOne(x => x.User)
            .WithOne(x => x.Student)
            .HasForeignKey<Student>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // School <-> SchoolMember <-> ApplicationUser
        builder.Entity<SchoolMember>()
            .HasKey(x => new { x.SchoolId, x.UserId });

        builder.Entity<SchoolMember>()
            .HasOne(x => x.School)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.SchoolId);

        builder.Entity<SchoolMember>()
            .HasOne(x => x.User)
            .WithMany(x => x.SchoolMemberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Student can select each school only once.
        builder.Entity<StudentSchoolAccount>()
            .HasIndex(x => new { x.SchoolId, x.StudentId })
            .IsUnique();

        builder.Entity<StudentSchoolAccount>()
            .HasIndex(x => new { x.SchoolId, x.StudentNumber })
            .IsUnique();

        builder.Entity<StudentSchoolAccount>()
            .HasOne(x => x.School)
            .WithMany(x => x.StudentAccounts)
            .HasForeignKey(x => x.SchoolId);

        builder.Entity<StudentSchoolAccount>()
            .HasOne(x => x.Student)
            .WithMany(x => x.SchoolAccounts)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Tuition ledger
        builder.Entity<BalanceEntry>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Entity<BalanceEntry>()
            .HasOne(x => x.RecordedByUser)
            .WithMany()
            .HasForeignKey(x => x.RecordedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Payments
        builder.Entity<Payment>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        // SQL-specific filtered unique index.
        builder.Entity<Payment>()
            .HasIndex(x => x.TransactionHash)
            .IsUnique()
            .HasFilter("\"TransactionHash\" IS NOT NULL");

        // One receipt per payment.
        builder.Entity<Receipt>()
            .HasIndex(x => x.PaymentId)
            .IsUnique();

        builder.Entity<Receipt>()
            .HasIndex(x => x.ReceiptNumber)
            .IsUnique();

        // Payment <-> BalanceEntry many-to-many with an amount.
        builder.Entity<PaymentAllocation>()
            .HasKey(x => new { x.PaymentId, x.BalanceEntryId });

        builder.Entity<PaymentAllocation>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        // School payment configurations.
        builder.Entity<SchoolPaymentConfiguration>()
            .HasOne(x => x.School)
            .WithMany(x => x.PaymentConfigurations)
            .HasForeignKey(x => x.SchoolId);

        // Invitation token hashes must be unique.
        builder.Entity<SchoolAdminInvitation>()
            .HasIndex(x => x.TokenHash)
            .IsUnique();
    }
}