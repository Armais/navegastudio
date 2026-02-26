using NavegaStudio.Areas.Escrow.Models;

namespace NavegaStudio.Data;

public static class EscrowDataSeeder
{
    public static void Seed(EscrowDbContext db)
    {
        if (db.Escrows.Any()) return;

        var buyer = "0xaaaa000000000000000000000000000000000001";
        var seller = "0xbbbb000000000000000000000000000000000002";
        var arbiter = "0xcccc000000000000000000000000000000000003";

        // Escrow 1: Funded (ready for seller to ship)
        var e1 = new EscrowTransaction
        {
            Buyer = buyer,
            Seller = seller,
            Arbiter = arbiter,
            Amount = 1.5m,
            ArbiterFeeBps = 100,
            Description = "Web3 Logo Design — 3 concepts + source files",
            State = EscrowState.Funded,
            CreatedAt = DateTime.UtcNow.AddHours(-2),
            UpdatedAt = DateTime.UtcNow.AddHours(-2)
        };
        e1.Events.Add(new EscrowEvent { Action = "Created", PerformedBy = buyer, Details = "Escrow created for 1.5 ETH", Timestamp = DateTime.UtcNow.AddHours(-2) });
        e1.Events.Add(new EscrowEvent { Action = "Funded", PerformedBy = buyer, Details = "Funded with 1.5 ETH", Timestamp = DateTime.UtcNow.AddHours(-2).AddSeconds(1) });

        // Escrow 2: Shipped (ready for buyer to confirm)
        var e2 = new EscrowTransaction
        {
            Buyer = buyer,
            Seller = seller,
            Arbiter = arbiter,
            Amount = 0.75m,
            ArbiterFeeBps = 200,
            Description = "Smart Contract Audit — ERC-20 token review",
            State = EscrowState.Shipped,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddHours(-6)
        };
        e2.Events.Add(new EscrowEvent { Action = "Created", PerformedBy = buyer, Details = "Escrow created for 0.75 ETH", Timestamp = DateTime.UtcNow.AddDays(-1) });
        e2.Events.Add(new EscrowEvent { Action = "Funded", PerformedBy = buyer, Details = "Funded with 0.75 ETH", Timestamp = DateTime.UtcNow.AddDays(-1).AddSeconds(1) });
        e2.Events.Add(new EscrowEvent { Action = "Shipped", PerformedBy = seller, Details = "Seller confirmed shipment", Timestamp = DateTime.UtcNow.AddHours(-6) });

        // Escrow 3: Completed
        var e3 = new EscrowTransaction
        {
            Buyer = buyer,
            Seller = seller,
            Arbiter = arbiter,
            Amount = 2.0m,
            ArbiterFeeBps = 150,
            Description = "DeFi Dashboard Development — React + Ethers.js frontend",
            State = EscrowState.Completed,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-2)
        };
        e3.Events.Add(new EscrowEvent { Action = "Created", PerformedBy = buyer, Details = "Escrow created for 2 ETH", Timestamp = DateTime.UtcNow.AddDays(-5) });
        e3.Events.Add(new EscrowEvent { Action = "Funded", PerformedBy = buyer, Details = "Funded with 2 ETH", Timestamp = DateTime.UtcNow.AddDays(-5).AddSeconds(1) });
        e3.Events.Add(new EscrowEvent { Action = "Shipped", PerformedBy = seller, Details = "Seller confirmed delivery", Timestamp = DateTime.UtcNow.AddDays(-3) });
        e3.Events.Add(new EscrowEvent { Action = "Completed", PerformedBy = buyer, Details = "Buyer confirmed receipt. Seller receives 1.97 ETH, arbiter fee 0.03 ETH", Timestamp = DateTime.UtcNow.AddDays(-2) });

        db.Escrows.AddRange(e1, e2, e3);
        db.SaveChanges();
    }
}
