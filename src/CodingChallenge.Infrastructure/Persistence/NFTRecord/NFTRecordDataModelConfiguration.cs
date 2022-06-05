using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodingChallenge.Infrastructure.Persistence.NFTRecord;
public class NFTRecordDataModelConfiguration : IEntityTypeConfiguration<NFTRecordDataModel>
{
    public void Configure(EntityTypeBuilder<NFTRecordDataModel> builder)
    {
        builder.HasKey(m => m.TokenId);
        builder.Property(t => t.TokenId)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(t => t.WalletId)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(t => t.Created)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(t => t.LastModifiedBy)
            .HasMaxLength(200);
        builder.Property(t => t.LastModified)
            .HasMaxLength(200);
    }
}