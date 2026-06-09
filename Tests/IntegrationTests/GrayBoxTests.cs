using System;
using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.IntegrationTests
{
    [TestFixture]
    public class GrayBoxTests
    {
        // TC-GB-01 | Sipariş sonrası sepet temizlenir | cart.IsEmpty = true
        [Test]
        public void TC_GB_01_CartShouldBeEmptyAfterOrder()
        {
            var service = new OrderService();
            var cart = new Cart();
            cart.AddItem(new Product(1, "Test", 100m, 10), 1);
            
            service.CreateOrder("C1", cart, PaymentMethod.CreditCard);

            Assert.IsTrue(cart.IsEmpty);
        }

        // TC-GB-02 | Kargodaki sipariş iptal edilemez | InvalidOperationException
        [Test]
        public void TC_GB_02_CannotCancelShippedOrder_ShouldThrowException()
        {
            var service = new OrderService();
            var cart = new Cart();
            cart.AddItem(new Product(2, "Test", 100m, 10), 1);
            var order = service.CreateOrder("C1", cart, PaymentMethod.CreditCard);
            order.Status = OrderStatus.Shipped; // internal state manipulation

            Assert.Throws<InvalidOperationException>(() => service.CancelOrder(order));
        }

        // TC-GB-03 | Negatif kupon reddedilmeli | ArgumentException
        [Test]
        public void TC_GB_03_NegativeCoupon_ShouldThrowException()
        {
            var cart = new Cart();
            cart.AddItem(new Product(3, "Test", 100m, 10), 1);
            
            Assert.Throws<ArgumentException>(() => cart.GetTotal(-50m));
        }

        // TC-GB-04 | Müşteri siparişleri doğru listelenmeli | Count = 2
        [Test]
        public void TC_GB_04_ListCustomerOrders_ShouldReturnCorrectCount()
        {
            var service = new OrderService();
            
            var cart1 = new Cart();
            cart1.AddItem(new Product(4, "Test1", 10m, 10), 1);
            service.CreateOrder("musteri-A", cart1, PaymentMethod.CreditCard);

            var cart2 = new Cart();
            cart2.AddItem(new Product(5, "Test2", 20m, 10), 1);
            service.CreateOrder("musteri-A", cart2, PaymentMethod.CreditCard);

            var orders = service.GetOrdersByCustomer("musteri-A");

            Assert.That(orders.Count, Is.EqualTo(2));
        }
    }
}
