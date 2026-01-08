using System.Windows;
using lab2_4.Entity;
using lab2_4.Request;

namespace lab2_4;

public partial class Login : Window
{
    public LoginEntity LoginEntity { get; set; }
    
    public Login()
    {
        InitializeComponent();
        LoginEntity = new LoginEntity("", "");
        DataContext = LoginEntity;
    }
    
    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (LoginEntity.name.Length == 0 || LoginEntity.password.Length == 0)
        {
            MessageBox.Show("Поля не можуть бути пусті!");    
        }else
        {
            try
            {
                bool loginRequest = await LoginRequest.Send(LoginEntity.name, LoginEntity.password);
                
                if (loginRequest)
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
    }

    private void OpenRegisterWindow_Click(object sender, RoutedEventArgs e)
    {
        var register = new Register();
        register.Show();
        Close();
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginEntity viewModel)
        {
            viewModel.password = PasswordBox.Password; 
        }
    }
}