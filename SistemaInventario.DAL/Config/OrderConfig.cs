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
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.AppUserId).IsRequired();
            builder.Property(x => x.OrderDate).IsRequired();
            builder.Property(x => x.TotalOrder).IsRequired();
            builder.Property(x => x.OrderState).IsRequired();
            builder.Property(x => x.PaymentState).IsRequired();
            builder.Property(x => x.ClientName).IsRequired();
            builder.Property(x => x.ShippingNumber).IsRequired(false);
            builder.Property(x => x.Carrier).IsRequired(false);
            builder.Property(x => x.TransactionId).IsRequired(false);
            builder.Property(x => x.Phone).IsRequired(false);
            builder.Property(x => x.Address).IsRequired(false);
            builder.Property(x => x.City).IsRequired(false);
            builder.Property(x => x.Country).IsRequired(false);
            builder.Property(x => x.SessionId).IsRequired(false);

            /*Relationship */

            builder.HasOne(x => x.AppUser).WithMany()
                .HasForeignKey(x => x.AppUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
