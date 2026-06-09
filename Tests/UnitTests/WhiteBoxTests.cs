using System;
using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.UnitTests
{
    [TestFixture]
    public class WhiteBoxTests
    {
        // TC-WB-01 | Negatif fiyatlı ürün oluşturma | ArgumentException fırlatılmalı
        [Test]
        public void TC_WB_01_CreateProductWithNegativePrice_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Product(1, "Test", -10m, 5));
        }

        // TC-WB-02 | Stoktan fazla miktar düşme | InvalidOperationException
        [Test]
        public void TC_WB_02_ReduceStockMoreThanAvailable_ShouldThrowException()
        {
            var product = new Product(2, "Test", 100m, 5);
            Assert.Throws<InvalidOperationException>(() => product.ReduceStock(12));
        }

        // TC-WB-03 | %100+ indirim uygulaması | ArgumentOutOfRangeException
        [Test]
        public void TC_WB_03_ApplyOver100PercentDiscount_ShouldThrowException()
        {
            var product = new Product(3, "Test", 100m, 10);
            Assert.Throws<ArgumentOutOfRangeException>(() => product.GetDiscountedPrice(150));
        }

        // TC-WB-04 | %20 indirim doğru hesaplama | 400 TL
        [Test]
        public void TC_WB_04_Calculate20PercentDiscount_ShouldReturn400()
        {
            var product = new Product(4, "Test", 500m, 10);
            decimal discounted = product.GetDiscountedPrice(20);
            Assert.That(discounted, Is.EqualTo(400m));
        }

        // TC-WB-05 | 100 TL kupon bir kez uygulanmalı | 900 TL
        [Test]
        public void TC_WB_05_Apply100TLCoupon_ShouldDeductOnce()
        {
            var cart = new Cart();
            cart.AddItem(new Product(5, "Test", 1000m, 10), 1);
            decimal total = cart.GetTotal(100m);
            Assert.That(total, Is.EqualTo(900m));
        }

        // TC-WB-06 | Aynı ürün duplicate eklenmemeli | Items.Count = 1, Qty = 2
        [Test]
        public void TC_WB_06_AddSameProductTwice_ShouldUpdateQuantityNotAddDuplicate()
        {
            var cart = new Cart();
            var product = new Product(6, "Test", 100m, 10);
            cart.AddItem(product, 1);
            cart.AddItem(product, 1);

            Assert.That(cart.Items.Count, Is.EqualTo(1));
            Assert.That(cart.Items[0].Quantity, Is.EqualTo(2));
        }
    }
}
