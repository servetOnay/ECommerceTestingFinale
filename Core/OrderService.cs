using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceApp.Core
{
    public enum OrderStatus { Pending, Confirmed, Cancelled, Shipped }
    public enum PaymentMethod { CreditCard, PayPal }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public Cart Cart { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public bool IsPaid { get; set; }
    }

    public class OrderService
    {
        private List<Order> _orders = new List<Order>();

        public Order CreateOrder(string customerId, Cart cart, PaymentMethod paymentMethod)
        {
            if (cart.IsEmpty)
            {
                throw new InvalidOperationException("Sepet bos.");
            }

            var order = new Order
            {
                Id = _orders.Count + 1,
                CustomerId = customerId,
                Cart = cart, // Simplified reference for tests
                TotalAmount = cart.GetTotal(),
                Status = OrderStatus.Pending,
                IsPaid = false
            };

            // Bug B7: Siparis verilince stok dusulmuyor
            // Normalde burada cart.Items uzerinde donup ReduceStock cagrilmaliydi.
            
            _orders.Add(order);
            cart.Clear();
            return order;
        }

        public bool ProcessPayment(Order order, decimal paymentAmount)
        {
            // Bug B6: Eksik odeme de siparisi Confirmed yapiyor
            if (paymentAmount > 0)
            {
                order.IsPaid = true;
                order.Status = OrderStatus.Confirmed;
                return true;
            }
            return false;
        }

        public void CancelOrder(Order order)
        {
            if (order.Status == OrderStatus.Shipped)
            {
                throw new InvalidOperationException("Kargodaki siparis iptal edilemez.");
            }
            order.Status = OrderStatus.Cancelled;
        }

        public List<Order> GetOrdersByCustomer(string customerId)
        {
            return _orders.Where(o => o.CustomerId == customerId).ToList();
        }
    }
}
