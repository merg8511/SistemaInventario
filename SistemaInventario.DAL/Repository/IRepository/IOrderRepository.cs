using SistemaInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DAL.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Update(Order order);
        void UpdateState(int id, string orderState, string paymentState);
        void UpdateStripePaymentId(int id, string sessionId, string transactionId);
    }
}
