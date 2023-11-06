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
    public class WarehouseProductConfig : IEntityTypeConfiguration<WarehouseProduct>
    {
        public void Configure(EntityTypeBuilder<WarehouseProduct> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.WarehouseId).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();

            //Relationship

            builder.HasOne(x => x.Warehouse).WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Product).WithMany()
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
