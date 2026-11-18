using System.Linq;
using System.Windows;
using PR1.Models;

namespace PR1.Windows;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e)
    {
        lblError.Visibility = Visibility.Collapsed;

        string login = tbLogin.Text.Trim();
        string password = tbPassword.Password;

        if (login.Length == 0 || password.Length == 0)
        {
            ShowError("Введите логин и пароль.");
            return;
        }

        using var db = new AppDbContext();
        var user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

        if (user == null)
        {
            ShowError("Неверный логин или пароль.");
            return;
        }

        App.CurrentUser = user;

        new MainWindow().Show();
        Close();
    }

    private void btnRegister_Click(object sender, RoutedEventArgs e)
    {
        var reg = new RegisterWindow { Owner = this };
        if (reg.ShowDialog() == true)
        {
            tbLogin.Text = reg.RegisteredLogin;
            tbPassword.Clear();
            tbLogin.Focus();
        }
    }

    private void ShowError(string message)
    {
        lblError.Text = message;
        lblError.Visibility = Visibility.Visible;
    }
}