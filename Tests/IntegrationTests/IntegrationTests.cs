using System;
using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.IntegrationTests
{
    [TestFixture]
    public class IntegrationTests
    {
        // TC-IT-01 | Tam alışveriş akışı (happy path) | Tüm adımlar başarılı
        [Test]
        public void TC_IT_01_FullShoppingFlow_ShouldSucceed()
        {
            var service = new OrderService();
            var cart = new Cart();
            var product = new Product(1, "Test", 100m, 10);
            
            cart.AddItem(product, 1);
            var order = service.CreateOrder("C1", cart, PaymentMethod.CreditCard);
            bool paid = service.ProcessPayment(order, 100m);

            Assert.IsTrue(paid);
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Confirmed));
            Assert.IsTrue(cart.IsEmpty);
        }

        // TC-IT-02 | Sipariş sonrası stok entegrasyonu | Stock = 7 (10-3)
        [Test]
        public void TC_IT_02_StockIntegrationAfterOrder_ShouldReduceStock()
        {
            var service = new OrderService();
            var cart = new Cart();
            var product = new Product(2, "Test", 100m, 10);
            
            cart.AddItem(product, 3);
            service.CreateOrder("C1", cart, PaymentMethod.CreditCard);

            Assert.That(product.Stock, Is.EqualTo(7));
        }

        // TC-IT-03 | Pending sipariş iptali | Status = Cancelled
        [Test]
        public void TC_IT_03_CancelPendingOrder_ShouldSetStatusToCancelled()
        {
            var service = new OrderService();
            var cart = new Cart();
            cart.AddItem(new Product(3, "Test", 100m, 10), 1);
            var order = service.CreateOrder("C1", cart, PaymentMethod.CreditCard);

            service.CancelOrder(order);

            Assert.That(order.Status, Is.EqualTo(OrderStatus.Cancelled));
        }

        // TC-IT-04 | Çoklu müşteri izolasyonu | Siparişler birbirini etkilemiyor
        [Test]
        public void TC_IT_04_MultiCustomerIsolation_OrdersShouldNotInterfere()
        {
            var service = new OrderService();
            
            var cart1 = new Cart();
            cart1.AddItem(new Product(4, "Test1", 10m, 10), 1);
            service.CreateOrder("C1", cart1, PaymentMethod.CreditCard);

            var cart2 = new Cart();
            cart2.AddItem(new Product(5, "Test2", 20m, 10), 1);
            service.CreateOrder("C2", cart2, PaymentMethod.CreditCard);

            var c1Orders = service.GetOrdersByCustomer("C1");
            var c2Orders = service.GetOrdersByCustomer("C2");

            Assert.That(c1Orders.Count, Is.EqualTo(1));
            Assert.That(c2Orders.Count, Is.EqualTo(1));
            Assert.AreNotEqual(c1Orders[0].Id, c2Orders[0].Id);
        }
    }
}
