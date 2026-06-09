using System;

namespace ECommerceApp.Core
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public Product(int id, string name, decimal price, int stock)
        {
            // Bug B1: Negatif fiyat/stok degerleri icin validation yok
            Id = id;
            Name = name;
            Price = price;
            Stock = stock;
        }

        public void ReduceStock(int quantity)
        {
            // Bug B2: Stok siniri kontrolu yok -> negatif stok olusuyor
            Stock -= quantity;
        }

        public decimal GetDiscountedPrice(double discountPercent)
        {
            // Bug B3: %100+ indirim engellenmiyor -> negatif fiyat
            return Price - (Price * (decimal)discountPercent / 100);
        }
    }
}
