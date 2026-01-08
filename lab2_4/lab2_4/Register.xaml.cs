using System.Windows;
using lab2_4.Entity;
using lab2_4.Request;

namespace lab2_4;

public partial class Register : Window
{
    public RegisterEntity RegisterEntity { get; set; }

    public Register()
    {
        InitializeComponent();
        RegisterEntity = new RegisterEntity("", "");
        DataContext = RegisterEntity;
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        if (RegisterEntity.name.Length == 0 || RegisterEntity.password.Length == 0)
        {
            MessageBox.Show("Поля не можуть бути пусті!");
        }
        else
        {
            try
            {
                var registerRequest = await RegisterRequest.Send(RegisterEntity.name, RegisterEntity.password);

                if (registerRequest)
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Сталася помилка {exception.Message}");
                throw;
            }
        }
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterEntity viewModel)
        {
            viewModel.password = PasswordBox.Password;
        }
    }   

    private void Close(object sender, RoutedEventArgs e)
    {
        var login = new Login();
        login.Show();
        Close();
    }
}