using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public string ShippingNumber { get; set; }
        public string Carrier { get; set; }

        [Required]
        public double TotalOrder { get; set; }

        [Required]
        public string OrderState { get; set; }

        public string PaymentState { get; set; }

        public DateTime PaymentDate { get; set; }
        public DateTime MaxPaymentDate { get; set; }

        //stripe 
        public string TransactionId { get; set; }
        public string SessionId { get; set; }
        //stripe 
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ClientName { get; set; }
    }
}
