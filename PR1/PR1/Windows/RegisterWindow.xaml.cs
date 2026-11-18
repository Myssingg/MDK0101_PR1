using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using PR1.Models;

namespace PR1.Windows;

public partial class RegisterWindow : Window
{
    // Маска телефона из задания
    private static readonly Regex PhoneRegex =
        new(@"^\+7 \d{3} \d{3}-\d{2}-\d{2}$");

    // "ул. Ленина, д. 5, кв. 12"; группа с кв. необязательна
    private static readonly Regex AddressRegex =
        new(@"^ул\.\s+(.+?),\s*д\.\s*(\d+[А-Яа-яA-Za-z]?)(?:,\s*кв\.\s*(\d+))?$");

    public string? RegisteredLogin { get; private set; }

    public RegisterWindow()
    {
        InitializeComponent();
    }

    private void btnRegister_Click(object sender, RoutedEventArgs e)
    {
        lblError.Visibility = Visibility.Collapsed;

        string fullName = tbFullName.Text.Trim();
        string login = tbLogin.Text.Trim();
        string password = tbPassword.Password;
        string phone = tbPhone.Text.Trim();
        string address = tbAddress.Text.Trim();

        if (fullName.Length == 0) { ShowError("Введите ФИО."); return; }
        if (login.Length < 3) { ShowError("Логин — не короче 3 символов."); return; }
        if (password.Length < 4) { ShowError("Пароль — не короче 4 символов."); return; }

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

        if (db.Users.Any(u => u.Login == login))
        {
            ShowError("Такой логин уже занят.");
            return;
        }

        var user = new User
        {
            FullName = fullName,
            Login = login,
            Password = password,
            Phone = phone,
            Street = street,
            House = house,
            Apartment = apartment
        };

        db.Users.Add(user);
        db.SaveChanges();

        RegisteredLogin = login;
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