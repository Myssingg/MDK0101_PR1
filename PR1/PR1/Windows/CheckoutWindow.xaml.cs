using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using PR1.Models;

namespace PR1.Windows;

public partial class CheckoutWindow : Window
{
    private static readonly Regex PhoneRegex =
        new(@"^\+7 \d{3} \d{3}-\d{2}-\d{2}$");

    private static readonly Regex AddressRegex =
        new(@"^ул\.\s+(.+?),\s*д\.\s*(\d+[А-Яа-яA-Za-z]?)(?:,\s*кв\.\s*(\d+))?$");

    public CheckoutWindow()
    {
        InitializeComponent();
        Prefill();
    }

    private void Prefill()
    {
        var user = App.CurrentUser;
        if (user == null) return;

        tbFullName.Text = user.FullName;
        tbPhone.Text = user.Phone;
        tbAddress.Text = user.Apartment == null
            ? $"ул. {user.Street}, д. {user.House}"
            : $"ул. {user.Street}, д. {user.House}, кв. {user.Apartment}";
        cbNoApartment.IsChecked = user.Apartment == null;

        using var db = new AppDbContext();
        var rows = db.Baskets
            .Include(b => b.Bouquet)
            .Where(b => b.UserId == user.Id)
            .ToList();

        decimal total = rows.Sum(r => r.Bouquet.Price * r.Quantity);
        lblTotal.Text = $"К оплате: {total:0} ₽";
    }

    private void btnSubmit_Click(object sender, RoutedEventArgs e)
    {
        lblError.Visibility = Visibility.Collapsed;

        var user = App.CurrentUser;
        if (user == null)
        {
            MessageBox.Show("Пользователь не авторизован.");
            return;
        }

        string fullName = tbFullName.Text.Trim();
        string phone = tbPhone.Text.Trim();
        string address = tbAddress.Text.Trim();

        if (fullName.Length == 0) { ShowError("Введите ФИО."); return; }

        if (!PhoneRegex.IsMatch(phone))
        {
            ShowError("Телефон в формате +7 XXX XXX-XX-XX.");
            return;
        }

        var m = AddressRegex.Match(address);
        if (!m.Success)
        {
            ShowError("Адрес в формате: ул. Ленина, д. 5, кв. 12");
            return;
        }

        string street = m.Groups[1].Value.Trim();
        string house = m.Groups[2].Value;
        int? apartment = m.Groups[3].Success ? int.Parse(m.Groups[3].Value) : null;
        bool noApartment = cbNoApartment.IsChecked == true;

        if (noApartment && apartment != null)
        {
            ShowError("Отмечено «Нет квартиры», но в адресе указана квартира.");
            return;
        }
        if (!noApartment && apartment == null)
        {
            ShowError("Укажите квартиру или отметьте «Нет квартиры».");
            return;
        }

        using var db = new AppDbContext();

        var cartRows = db.Baskets
            .Include(b => b.Bouquet)
            .Where(b => b.UserId == user.Id)
            .ToList();

        if (cartRows.Count == 0)
        {
            MessageBox.Show("Корзина пуста.");
            return;
        }

        // Отредактированные контакты сохраняем в профиль
        var profile = db.Users.First(u => u.Id == user.Id);
        profile.FullName = fullName;
        profile.Phone = phone;
        profile.Street = street;
        profile.House = house;
        profile.Apartment = apartment;

        var order = new Order
        {
            UserId = user.Id,
            CreatedAt = DateTime.Now,
            DeliveryTime = DateTime.Now.AddHours(2),
            DeliveryStreet = street,
            DeliveryHouse = house,
            DeliveryApartment = apartment,
            TotalCost = cartRows.Sum(r => r.Bouquet.Price * r.Quantity)
        };

        foreach (var row in cartRows)
        {
            order.OrderItems.Add(new OrderItem
            {
                BouquetId = row.BouquetId,
                Quantity = row.Quantity,
                UnitPrice = row.Bouquet.Price
            });
        }

        db.Orders.Add(order);
        db.Baskets.RemoveRange(cartRows);
        db.SaveChanges();

        App.CurrentUser = profile;

        MessageBox.Show($"Заказ №{order.Id} оформлен. Сумма: {order.TotalCost:0} ₽");
        DialogResult = true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ShowError(string message)
    {
        lblError.Text = message;
        lblError.Visibility = Visibility.Visible;
    }
}