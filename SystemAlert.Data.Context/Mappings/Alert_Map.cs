using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemAlert.Data.Context.Models;

namespace SystemAlert.Data.Context.Mappings
{
    public class Alert_Map : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.HasKey(x => x.Id).IsClustered(false);

            builder.HasIndex(x => x.CreatedOn).IsClustered(true);

            builder.HasIndex(x => new { x.AlertTypeId, x.CustomerId })
                .HasDatabaseName("IX_Alert_AlertTypeId");

            builder.HasIndex(x => new { x.CustomerId, x.ResourceId, x.AlertTypeId, x.ResolvedOn })
                .HasDatabaseName("IX_Alert_ResourceId_AlertTypeId_ResolvedOn");

            builder.Property(x => x.Id)
                .HasDefaultValueSql("(newid())");

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.AlertTypeId).HasMaxLength(128).IsRequired();
            builder.Property(x => x.ResourceId).HasMaxLength(256).IsRequired();
            builder.Property(x => x.Content).HasColumnType("nvarchar(max)").IsRequired();

            builder.Property(x => x.CreatedOn)
                .HasColumnType("Datetime2(7)")
                .HasDefaultValueSql("(GetDate())")
                .IsRequired();

            builder.Property(x => x.SentOn)
                .HasColumnType("Datetime2(7)")
                .IsRequired(false);

            builder.Property(x => x.ResolvedBy)
                .IsRequired(false);

            builder.Property(x => x.ResolvedOn)
                .HasColumnType("Datetime2(7)")
                .IsRequired(false);

            builder.HasOne<AlertType>().WithMany()
                .HasForeignKey(x => x.AlertTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
