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
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.SerieNumber).IsRequired().HasMaxLength(60);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(100);
            builder.Property(x => x.pState).IsRequired();
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.Cost).IsRequired();
            builder.Property(x => x.CategoryId).IsRequired();
            builder.Property(x => x.BrandId).IsRequired();
            builder.Property(x => x.UrlImage).IsRequired(false);
            builder.Property(x => x.ParentId).IsRequired(false);

            /*Relationship*/

            builder.HasOne(x => x.Category).WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Brand).WithMany()
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Parent).WithMany()
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
