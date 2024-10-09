using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PeShopMaster.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private int failedAttempts = 0;
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                if (string.IsNullOrEmpty(LoginBox.Text))
                {
                    errors.AppendLine("Заполните логин");
                }
                if (string.IsNullOrEmpty(PasswordTextBox.Password))
                {
                    errors.AppendLine("Заполните пароль");
                }

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (failedAttempts > 0)
                {
                    if (string.IsNullOrEmpty(WriteCapcha.Text))
                    {
                        MessageBox.Show("Введите капчу", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        LoadCaptcha();
                        return;
                    }

                    if (!WriteCapcha.Text.Equals(CapchaBox.Text, StringComparison.Ordinal))
                    {
                        MessageBox.Show("Неверная капча!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                        WriteCapcha.Text = "";
                        LoadCaptcha();
                        return;
                    }
                }

                if (Data.Trade2Entities.GetContext().User
                    .Any(d => d.UserLogin == LoginBox.Text &&
                    d.UserPassword == PasswordTextBox.Password))
                {
                    var user = Data.Trade2Entities.GetContext().User.Where(d => d.UserLogin ==
                    LoginBox.Text &&
                    d.UserPassword == PasswordTextBox.Password).FirstOrDefault();
                    Classes.Manager.User = user;
                    switch (user.Role.RoleName)
                    {
                        case "Администратор":
                            Classes.Manager.MainFrame.Navigate(new Pages.AdminPage());
                            break;
                        case "Клиент":
                            Classes.Manager.MainFrame.Navigate(new Pages.ClientPage());
                            break;
                        case "Менеджер":
                            Classes.Manager.MainFrame.Navigate(new Pages.ManagerPage());
                            break;
                    }
                    MessageBox.Show("Успех", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    Capcha.Visibility = Visibility.Collapsed;
                    CapchaBox.Visibility = Visibility.Collapsed;
                    WriteCapcha.Visibility = Visibility.Collapsed;
                    WriteCapcha.Text = "";
                    failedAttempts = 0;
                }
                else
                {
                    MessageBox.Show("Учетная запись не найдена", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    failedAttempts++;
                    LoadCaptcha();
                    ShowCaptchaFields();
                    if (failedAttempts > 1)
                    {
                        LoginButton.IsEnabled = false;
                        await Task.Delay(10000);
                        LoginButton.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCaptcha()
        {
            string allowChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            string captcha = "";
            Random random = new Random();

            for (int i = 0; i < 4; i++)
            {
                captcha += allowChars[random.Next(allowChars.Length)];
            }

            CapchaBox.Text = captcha;
        }

        private void ShowCaptchaFields()
        {
            Capcha.Visibility = Visibility.Visible;
            CapchaBox.Visibility = Visibility.Visible;
            WriteCapcha.Visibility = Visibility.Visible;
        }
        private void QuestButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
