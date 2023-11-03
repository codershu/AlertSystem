using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemAlert.Data.Context.Models;

namespace SystemAlert.Data.Context.Mappings
{
    public class AlertConfiguration_Map : IEntityTypeConfiguration<AlertConfiguration>
    {
        public void Configure(EntityTypeBuilder<AlertConfiguration> builder)
        {
            builder.ToTable("AlertConfigurations", x => x.IsTemporal());

            builder.HasKey(x => x.Id).IsClustered(false);

            builder.HasIndex(x => new { x.AlertTypeId, x.CustomerId })
                .HasDatabaseName("IX_AlertConfiguration_AlertTypeId");

            builder.HasIndex(x => new { x.CustomerId, x.SendTo, x.AlertTypeId })
                .HasDatabaseName("IX_AlertConfiguration_SentTo_AlertTypeId");

            builder.Property(x => x.Id)
                .HasDefaultValueSql("(newid())");

            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.AlertTypeId).HasMaxLength(128).IsRequired();
            builder.Property(x => x.SendTo).HasMaxLength(32).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(512).IsRequired();
            builder.Property(x => x.CoolDown).IsRequired();
            builder.Property(x => x.CreatedBy).IsRequired();
            builder.Property(x => x.UpdatedBy).IsRequired();

            builder.HasOne<AlertType>().WithMany()
                .HasForeignKey(x => x.AlertTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
