
using Microsoft.EntityFrameworkCore;

namespace CodingChallenge.Infrastructure.Persistence.NFTRecord;


public class NFTRecordDataModelDbContext : DbContext
{
    public NFTRecordDataModelDbContext(DbContextOptions<NFTRecordDataModelDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new NFTRecordDataModelConfiguration());
    }

    public DbSet<NFTRecordDataModel> NftDataModel { get; set; }
}
