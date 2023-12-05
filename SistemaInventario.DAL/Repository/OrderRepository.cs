using SistemaInventario.DAL.Data;
using SistemaInventario.DAL.Repository.IRepository;
using SistemaInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.DAL.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Order order)
        {
            _dbContext.Update(order);
        }

        public void UpdateState(int id, string orderState, string paymentState)
        {
            var order = _dbContext.Orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                order.OrderState = orderState;
                order.PaymentState = paymentState;
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string transactionId)
        {
            var order = _dbContext.Orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                if (!String.IsNullOrEmpty(sessionId))
                {
                    order.SessionId = sessionId;
                }
                if (!String.IsNullOrEmpty(transactionId))
                {
                    order.TransactionId = transactionId;
                    order.PaymentDate = DateTime.Now;
                }
            }
        }
    }
}
