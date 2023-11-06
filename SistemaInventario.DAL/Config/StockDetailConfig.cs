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
    public class StockDetailConfig : IEntityTypeConfiguration<StockDetail>
    {
        public void Configure(EntityTypeBuilder<StockDetail> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.StockId).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.LastStock).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();

            //Relationship

            builder.HasOne(x => x.Stock).WithMany()
                .HasForeignKey(x => x.StockId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Product).WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
