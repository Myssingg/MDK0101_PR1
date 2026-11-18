using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PR1.Models;

namespace PR1.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        lblUser.Text = $"Вы вошли как {App.CurrentUser?.FullName}";
        LoadBouquets();
        UpdatebtnCart();
    }

    private void LoadBouquets()
    {
        using var db = new AppDbContext();

        var cards = db.Bouquets
            .OrderBy(b => b.Id)
            .ToList()
            .Select(b => new BouquetCard
            {
                Id = b.Id,
                Name = b.Name,
                PriceText = $"{b.Price:0} ₽",
                Image = LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, b.ImagePath))
            })
            .ToList();

        lstBouquets.ItemsSource = cards;
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

    private void lstBouquets_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        // щелчок по пустому месту не должен добавлять прошлый выбранный букет
        if (ItemsControl.ContainerFromElement(lstBouquets, (DependencyObject)e.OriginalSource) == null)
            return;

        if (lstBouquets.SelectedItem is not BouquetCard card) return;

        using var db = new AppDbContext();
        int userId = App.CurrentUser!.Id;

        var row = db.Baskets.FirstOrDefault(x => x.UserId == userId && x.BouquetId == card.Id);
        if (row == null)
        {
            db.Baskets.Add(new Basket { UserId = userId, BouquetId = card.Id, Quantity = 1 });
        }
        else
        {
            row.Quantity += 1;
        }

        db.SaveChanges();
        UpdatebtnCart();
    }

    private void UpdatebtnCart()
    {
        using var db = new AppDbContext();
        int userId = App.CurrentUser!.Id;
        int count = db.Baskets.Where(x => x.UserId == userId).Sum(x => (int?)x.Quantity) ?? 0;
        btnCart.Content = count == 0 ? "Корзина" : $"Корзина ({count})";
    }

    private void btnCart_Click(object sender, RoutedEventArgs e)
    {
        var cart = new CartWindow { Owner = this };
        cart.ShowDialog();
        UpdatebtnCart();   // вернулись из корзины — обновить счётчик
    }

    private void btnLogout_Click(object sender, RoutedEventArgs e)
    {
        new LoginWindow().Show();
        Close();
    }
}

public class BouquetCard
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string PriceText { get; set; } = "";
    public BitmapImage? Image { get; set; }
}