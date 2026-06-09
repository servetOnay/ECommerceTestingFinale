# E-Ticaret Sistemi - Test Raporu

**Proje:** ECommerceApp  
**Test Framework:** NUnit 4.x   
**Toplam Test:** 20 | Pass: 11 | Fail: 9 | **Basari Orani: %55**

---

## Proje Yapisi

```text
ECommerceApp/
├── Core/
│   ├── Product.cs          <- Urun modeli
│   ├── Cart.cs             <- Sepet 
│   └── OrderService.cs     <- Siparis servisi 
├── Tests/
│   ├── UnitTests/
│   │   ├── WhiteBoxTests.cs    <- 6 test 
│   │   ├── BlackBoxTests.cs    <- 6 test 
│   │   └── GrayBoxTests.cs     <- 4 test 
│   └── IntegrationTests/
│       └── IntegrationTests.cs <- 4 test
└── Program.cs
```

---

## Sistemdeki Kasitli Hatalar (Bugs)

| # | Dosya | Metot | Hata Aciklamasi |
|---|-------|-------|-----------------|
| B1 | `Product.cs` | Constructor | Negatif fiyat/stok degerleri icin validation yok |
| B2 | `Product.cs` | `ReduceStock()` | Stok siniri kontrolu yok -> negatif stok olusuyor |
| B3 | `Product.cs` | `GetDiscountedPrice()` | %100+ indirim engellenmiyor -> negatif fiyat |
| B4 | `Cart.cs` | `AddItem()` | Ayni urun tekrar eklenince miktar guncellenmez, duplicate olusur |
| B5 | `Cart.cs` | `GetTotal()` | Kupon indirimi iki kez uygulaniyor (cifte indirim) |
| B6 | `OrderService.cs` | `ProcessPayment()` | Eksik odeme de siparisi Confirmed yapiyor |
| B7 | `OrderService.cs` | `CreateOrder()` | Siparis verilince stok dusulmuyor |

---

## 1. WHITE BOX TEST SONUCLARI

> Ic implementasyona tam erisimle yapilan testler. Hangi satirin hatali oldugunu bilerek hedefliyoruz.

| Test ID | Test Adi | Beklenen | Gercek Sonuc | Durum | Fail Nedeni |
|---------|----------|----------|--------------|-------|-------------|
| TC-WB-01 | Negatif fiyatli urun olusturma | `ArgumentException` firlatilmali | Exception yok, urun olustu | **FAIL** | Constructor'da `Price < 0` kontrolu yok |
| TC-WB-02 | Stoktan fazla miktar dusme | `InvalidOperationException` | Exception yok, Stock = -7 | **FAIL** | `ReduceStock()` icinde `quantity > Stock` kosulu yok |
| TC-WB-03 | %100+ indirim uygulamasi | `ArgumentOutOfRangeException` | Exception yok, `-100 TL` dondu | **FAIL** | `GetDiscountedPrice()` yuzde araligi kontrolu yok |
| TC-WB-04 | %20 indirim dogru hesaplama | `400 TL` | `400 TL` | **PASS** | - |
| TC-WB-05 | 100 TL kupon bir kez uygulanmali | `900 TL` | `800 TL` | **FAIL** | `GetTotal()` icinde `couponDiscount` iki kez cikariliyor |
| TC-WB-06 | Ayni urun duplicate eklenmemeli | `Items.Count = 1, Qty = 2` | `Items.Count = 2` | **FAIL** | `AddItem()` mevcut item kontrolu yapmiyor |

**Ozet:** 6 testten 4'u FAIL

---

## 2. BLACK BOX TEST SONUCLARI

> Kullanici bakis acisi: Sadece girdi/cikti. Ic koda bakilmaz.

| Test ID | Test Adi | Beklenen | Gercek Sonuc | Durum | Fail Nedeni |
|---------|----------|----------|--------------|-------|-------------|
| TC-BB-01 | Stoklu urun sepete ekleme | Basarili ekleme | Basarili ekleme | **PASS** | - |
| TC-BB-02 | Stoksuz urun sepete eklenemez | `InvalidOperationException` | Exception firlatildi | **PASS** | - |
| TC-BB-03 | Bos sepet ile siparis verilemez | `InvalidOperationException` | Exception firlatildi | **PASS** | - |
| TC-BB-04 | Tam odeme -> Confirmed statusu | `Status = Confirmed` | `Status = Confirmed` | **PASS** | - |
| TC-BB-05 | Eksik odeme -> Confirmed olmamali | `Status != Confirmed` | `Status = Confirmed` | **FAIL** | `ProcessPayment()` sadece `> 0` kontrol ediyor, tutar karsilastirmasi yok |
| TC-BB-06 | Siparis sonrasi stok azalmali | `Stock = 3` (5-2) | `Stock = 5` | **FAIL** | `CreateOrder()` stok dusme islemi yapmiyor |

**Ozet:** 6 testten 2'si FAIL

---

## 3. GRAY BOX TEST SONUCLARI

> Kismi ic bilgi: Metodlarin varligini ve veri yapilarini biliyoruz.

| Test ID | Test Adi | Beklenen | Gercek Sonuc | Durum | Fail Nedeni |
|---------|----------|----------|--------------|-------|-------------|
| TC-GB-01 | Siparis sonrasi sepet temizlenir | `cart.IsEmpty = true` | `true` | **PASS** | - |
| TC-GB-02 | Kargodaki siparis iptal edilemez | `InvalidOperationException` | Exception firlatildi | **PASS** | - |
| TC-GB-03 | Negatif kupon reddedilmeli | `ArgumentException` | Exception yok, hesaplama devam etti | **FAIL** | `GetTotal()` negatif kupon parametresini validate etmiyor |
| TC-GB-04 | Musteri siparisleri dogru listelenmeli | `Count = 2` (musteri-A) | `Count = 2` | **PASS** | - |

**Ozet:** 4 testten 1'i FAIL

---

## 4. INTEGRATION TEST SONUCLARI

> Uctan uca sistem akislari test edildi.

| Test ID | Test Adi | Beklenen | Gercek Sonuc | Durum | Fail Nedeni |
|---------|----------|----------|--------------|-------|-------------|
| TC-IT-01 | Tam alisveris akisi (happy path) | Tum adimlar basarili | Basarili | **PASS** | - |
| TC-IT-02 | Siparis sonrasi stok entegrasyonu | `Stock = 7` (10-3) | `Stock = 10` | **FAIL** | `CreateOrder()` `ReduceStock()` cagirmiyor - KRITIK |
| TC-IT-03 | Pending siparis iptali | `Status = Cancelled` | `Status = Cancelled` | **PASS** | - |
| TC-IT-04 | Coklu musteri izolasyonu | Siparisler birbirini etkilemiyor | Izole calisiyor | **PASS** | - |

**Ozet:** 4 testten 1'i FAIL

---

## Genel Ozet Tablosu

| Test Turu | Toplam | Pass | Fail | Basari Orani |
|-----------|--------|------|------|-------------|
| White Box | 6 | 2 | 4 | %33 |
| Black Box | 6 | 4 | 2 | %67 |
| Gray Box | 4 | 3 | 1 | %75 |
| Integration | 4 | 3 | 1 | %75 |
| **TOPLAM** | **20** | **11** | **9** | **%55** |

> Not: TC-IT-02 ve TC-BB-06 ayni bug'i (B7) farkli katmanlarda test ediyor.

---

## Duzeltme Onerileri

### Bug B1 - Product Constructor Validation
```csharp
// Mevcut (hatali)
public Product(int id, string name, decimal price, int stock, ...)
{
    Price = price;
    Stock = stock;
}

// Duzeltilmis
if (price < 0)  throw new ArgumentException("Fiyat negatif olamaz.", nameof(price));
if (stock < 0)  throw new ArgumentException("Stok negatif olamaz.", nameof(stock));
Price = price;
Stock = stock;
```

### Bug B2 - ReduceStock Sinir Kontrolu
```csharp
// Duzeltilmis
public void ReduceStock(int quantity)
{
    if (quantity > Stock)
        throw new InvalidOperationException($"Yetersiz stok. Mevcut: {Stock}, Istenen: {quantity}");
    Stock -= quantity;
}
```

### Bug B3 - GetDiscountedPrice Aralik Kontrolu
```csharp
// Duzeltilmis
public decimal GetDiscountedPrice(double discountPercent)
{
    if (discountPercent < 0 || discountPercent > 100)
        throw new ArgumentOutOfRangeException(nameof(discountPercent), "Indirim 0-100 arasinda olmali.");
    return Price - (Price * (decimal)discountPercent / 100);
}
```

### Bug B4 - AddItem Duplicate Kontrolu
```csharp
// Duzeltilmis
public void AddItem(Product product, int quantity)
{
    var existing = _items.FirstOrDefault(i => i.Product.Id == product.Id);
    if (existing != null)
        existing.Quantity += quantity;
    else
        _items.Add(new CartItem(product, quantity));
}
```

### Bug B5 - GetTotal Cifte Indirim
```csharp
// Duzeltilmis
public decimal GetTotal(decimal couponDiscount = 0)
{
    if (couponDiscount < 0) throw new ArgumentException("Kupon indirimi negatif olamaz.");
    decimal total = _items.Sum(i => i.Product.Price * i.Quantity) - couponDiscount;
    return total < 0 ? 0 : total;
}
```

### Bug B6 - ProcessPayment Tutar Kontrolu
```csharp
// Duzeltilmis
public bool ProcessPayment(Order order, decimal paymentAmount)
{
    if (paymentAmount < order.TotalAmount) return false; // Eksik odeme reddedilir
    order.IsPaid = true;
    order.Status = OrderStatus.Confirmed;
    return true;
}
```

### Bug B7 - CreateOrder Stok Dusme
```csharp
// Duzeltilmis
public Order CreateOrder(Cart cart, PaymentMethod paymentMethod)
{
    // ... mevcut kod ...
    foreach (var item in cart.Items)
        item.Product.ReduceStock(item.Quantity); // EKSIK SATIR EKLENDI
    _orders.Add(order);
    cart.Clear();
    return order;
}
```

---

## Test Metodolojisi Ozeti

### White Box
Kodun icine bakarak zayif noktalari direkt hedefledik. `ReduceStock`, `GetDiscountedPrice`, `GetTotal` ve constructor gibi metodlarin ic implementasyonunu bilerek sinir degerlerini test ettik.

### Black Box
Kullanici senaryolarini taklit ettik: "Sepete ekle -> Siparis ver -> Ode." Kodun nasil calistigini bilmeden beklenen davranislari test ettik. Iki kritik kullanici senaryosunda (eksik odeme, stok takibi) bug yakaladik.

### Gray Box
Sistemin veri modelini ve temel metodlari bilerek ancak implementasyon detaylarina bakmadan test ettik. Ozellikle sinir durumlarina (negatif kupon, iptal kosullari) odaklandik.

### Integration
Tum katmanlarin (Product + Cart + OrderService) birlikte calistigi end-to-end senaryolari test ettik. En kritik entegrasyon hatasi: stok yonetiminin siparis servisiyle entegre olmamasi.

---
