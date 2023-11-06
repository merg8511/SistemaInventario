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
    public class StockConfig : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.WarehouseId).IsRequired();
            builder.Property(x => x.AppUserId).IsRequired();
            builder.Property(x => x.InitDate).IsRequired();
            builder.Property(x => x.FinalDate).IsRequired();
            builder.Property(x => x.State).IsRequired();

            //Relationship

            builder.HasOne(x => x.Warehouse).WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.AppUser).WithMany()
                .HasForeignKey(x => x.AppUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
