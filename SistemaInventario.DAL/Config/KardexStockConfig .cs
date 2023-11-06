using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DAL.Config
{
    public class KardexStockConfig : IEntityTypeConfiguration<KardexStock>
    {
        public void Configure(EntityTypeBuilder<KardexStock> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.WarehouseProductId).IsRequired();
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Detail).IsRequired();
            builder.Property(x => x.LastStock).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();
            builder.Property(x => x.Cost).IsRequired();
            builder.Property(x => x.Stock).IsRequired();
            builder.Property(x => x.Total).IsRequired();
            builder.Property(x => x.AppUserId).IsRequired();
            builder.Property(x => x.RecordDate).IsRequired();

            //Relationship

            builder.HasOne(x => x.WarehouseProduct).WithMany()
                .HasForeignKey(x => x.WarehouseProductId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.AppUser).WithMany()
                .HasForeignKey(x => x.AppUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
