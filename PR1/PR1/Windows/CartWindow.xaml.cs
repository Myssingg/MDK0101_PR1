using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using PR1.Models;

namespace PR1.Windows;

public partial class CartWindow : Window
{
    public CartWindow()
    {
        InitializeComponent();
        LoadCart();
    }

    private void LoadCart()
    {
        using var db = new AppDbContext();
        int userId = App.CurrentUser!.Id;

        var rows = db.Baskets
            .Include(b => b.Bouquet)
            .Where(b => b.UserId == userId)
            .OrderBy(b => b.Id)
            .ToList();

        var items = rows.Select(b => new CartItem
        {
            BasketId = b.Id,
            Name = b.Bouquet.Name,
            Price = b.Bouquet.Price,
            Quantity = b.Quantity,
            Image = LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           b.Bouquet.ImagePath))
        }).ToList();

        lstCart.ItemsSource = items;

        decimal total = items.Sum(i => i.Price * i.Quantity);
        lblTotal.Text = $"Итого: {total:0} ₽";
        lblEmpty.Visibility = items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private static BitmapImage? LoadImage(string path)
    {
        if (!File.Exists(path)) return null;

        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = new Uri(path);
        image.EndInit();
        return image;
    }

    private void btnPlus_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not CartItem item) return;
        ChangeQuantity(item.BasketId, +1);
    }

    private void btnMinus_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not CartItem item) return;
        ChangeQuantity(item.BasketId, -1);
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not CartItem item) return;

        using var db = new AppDbContext();
        var row = db.Baskets.FirstOrDefault(x => x.Id == item.BasketId);
        if (row != null)
        {
            db.Baskets.Remove(row);
            db.SaveChanges();
        }

        LoadCart();
    }

    private void ChangeQuantity(int basketId, int delta)
    {
        using var db = new AppDbContext();
        var row = db.Baskets.FirstOrDefault(x => x.Id == basketId);
        if (row == null) return;

        row.Quantity += delta;

        if (row.Quantity <= 0)
        {
            db.Baskets.Remove(row);
        }

        db.SaveChanges();
        LoadCart();
    }

    private void btnCheckout_Click(object sender, RoutedEventArgs e)
    {
        var checkout = new CheckoutWindow { Owner = this };
        if (checkout.ShowDialog() == true)
        {
            DialogResult = true; 
        }
    }
}

public class CartItem
{
    public int BasketId { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public BitmapImage? Image { get; set; }

    public string PriceText => $"{Price:0} ₽";
    public string SumText => $"{Price * Quantity:0} ₽";
}