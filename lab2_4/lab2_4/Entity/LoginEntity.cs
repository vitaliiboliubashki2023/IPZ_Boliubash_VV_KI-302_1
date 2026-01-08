using System.ComponentModel;

namespace lab2_4.Entity;

public class LoginEntity : INotifyPropertyChanged
{
    private string _name;
    private string _password;
    public static int userId;

    public string name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(name));
            }
        }
    }

    public string password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                OnPropertyChanged(nameof(password));
            }
        }
    }

    public LoginEntity(string name, string password)
    {
        this._name = name;
        this._password = password;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}