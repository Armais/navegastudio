using Microsoft.EntityFrameworkCore;
using NavegaStudio.Areas.Escrow.Models;

namespace NavegaStudio.Data;

public class EscrowDbContext : DbContext
{
    public EscrowDbContext(DbContextOptions<EscrowDbContext> options) : base(options) { }

    public DbSet<EscrowTransaction> Escrows => Set<EscrowTransaction>();
    public DbSet<EscrowEvent> EscrowEvents => Set<EscrowEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EscrowTransaction>(entity =>
        {
            entity.HasIndex(e => e.Buyer);
            entity.HasIndex(e => e.Seller);
            entity.HasIndex(e => e.State);
            entity.Property(e => e.Amount).HasPrecision(18, 8);
            entity.Property(e => e.State).HasConversion<string>();
        });

        modelBuilder.Entity<EscrowEvent>(entity =>
        {
            entity.HasIndex(e => e.EscrowTransactionId);
            entity.HasOne(e => e.EscrowTransaction)
                  .WithMany(t => t.Events)
                  .HasForeignKey(e => e.EscrowTransactionId);
        });
    }
}
