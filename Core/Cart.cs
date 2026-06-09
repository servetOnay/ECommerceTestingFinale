using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceApp.Core
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }

    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public bool IsEmpty => Items.Count == 0;

        public void AddItem(Product product, int quantity)
        {
            if (product.Stock < quantity)
            {
                throw new InvalidOperationException("Yetersiz stok.");
            }

            // Bug B4: Ayni urun tekrar eklenince miktar guncellenmez, duplicate olusur
            Items.Add(new CartItem(product, quantity));
        }

        public decimal GetTotal(decimal couponDiscount = 0)
        {
            // Gray Box Test Bug: Negatif kupon degerlerini kontrol etmiyoruz (TC-GB-03)
            
            // Bug B5: Kupon indirimi iki kez uygulaniyor (cifte indirim)
            decimal total = Items.Sum(i => i.Product.Price * i.Quantity) - (couponDiscount * 2);
            return total < 0 ? 0 : total;
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}
