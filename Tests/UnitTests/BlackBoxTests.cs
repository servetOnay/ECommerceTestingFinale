using System;
using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.UnitTests
{
    [TestFixture]
    public class BlackBoxTests
    {
        // TC-BB-01 | Stoklu ürün sepete ekleme | Başarılı ekleme
        [Test]
        public void TC_BB_01_AddInStockProductToCart_ShouldSucceed()
        {
            var cart = new Cart();
            var product = new Product(1, "Test", 100m, 10);
            Assert.DoesNotThrow(() => cart.AddItem(product, 1));
            Assert.That(cart.Items.Count, Is.GreaterThan(0));
        }

        // TC-BB-02 | Stoksuz ürün sepete eklenemez | InvalidOperationException
        [Test]
        public void TC_BB_02_AddOutOfStockProduct_ShouldThrowException()
        {
            var cart = new Cart();
            var product = new Product(2, "Test", 100m, 0);
            Assert.Throws<InvalidOperationException>(() => cart.AddItem(product, 1));
        }

        // TC-BB-03 | Boş sepet ile sipariş verilemez | InvalidOperationException
        [Test]
        public void TC_BB_03_CreateOrderWithEmptyCart_ShouldThrowException()
        {
            var service = new OrderService();
            var cart = new Cart();
            Assert.Throws<InvalidOperationException>(() => service.CreateOrder("C1", cart, PaymentMethod.CreditCard));
        }

        // TC-BB-04 | Tam ödeme -> Confirmed statüsü | Status = Confirmed
        [Test]
        public void TC_BB_04_FullPayment_ShouldConfirmOrder()
        {
            var service = new OrderService();
            var cart = new Cart();
            cart.AddItem(new Product(3, "Test", 100m, 10), 1);
            var order = service.CreateOrder("C1", cart, PaymentMethod.CreditCard);

            bool success = service.ProcessPayment(order, 100m);

            Assert.IsTrue(success);
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Confirmed));
        }

        // TC-BB-05 | Eksik ödeme -> Confirmed olmamalı | Status ≠ Confirmed
        [Test]
        public void TC_BB_05_PartialPayment_ShouldNotConfirmOrder()
        {
            var service = new OrderService();
            var cart = new Cart();
            cart.AddItem(new Product(4, "Test", 100m, 10), 1);
            var order = service.CreateOrder("C1", cart, PaymentMethod.CreditCard);

            service.ProcessPayment(order, 50m); // Partial payment

            Assert.That(order.Status, Is.Not.EqualTo(OrderStatus.Confirmed));
        }

        // TC-BB-06 | Sipariş sonrası stok azalmalı | Stock = 3 (5-2)
        [Test]
        public void TC_BB_06_StockShouldDecreaseAfterOrder()
        {
            var service = new OrderService();
            var cart = new Cart();
            var product = new Product(5, "Test", 100m, 5);
            cart.AddItem(product, 2);
            service.CreateOrder("C1", cart, PaymentMethod.CreditCard);

            Assert.That(product.Stock, Is.EqualTo(3));
        }
    }
}
