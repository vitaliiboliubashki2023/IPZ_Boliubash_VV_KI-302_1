namespace lab2_4.Entity;

using System.ComponentModel;

public class ReceiptViewModel : INotifyPropertyChanged
{
    private string _orderNumber;
    private string _username;

    public string OrderNumber
    {
        get => _orderNumber;
        set
        {
            _orderNumber = value;
            OnPropertyChanged(nameof(OrderNumber));
        }
    }

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}