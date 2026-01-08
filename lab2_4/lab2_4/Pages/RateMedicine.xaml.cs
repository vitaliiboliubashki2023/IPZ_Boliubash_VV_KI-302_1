using lab2_4.Entity;

namespace lab2_4.Pages;

using System.Windows;

public partial class RateMedicine : Window
{
    public string Username { get; set; }
    public MedicineData Medicine { get; set; }

    public RateMedicine(Item item)
    {
        Medicine = new MedicineData(item.Name, 0);
        InitializeComponent();

        DataContext = Medicine;
    }

    private void Close(object sender, RoutedEventArgs e)
    {
        var main = new MainWindow();
        main.Show();
        Close();
    }

    private void SendClick(object sender, RoutedEventArgs e)
    {
        MessageBox.Show($"Оцінено {Medicine.Name} на {Medicine.Rating}");
        var main = new MainWindow();
        main.Show();
        Close();
    }
}

public class MedicineData
{
    public string Name { get; set; }
    public decimal Rating { get; set; }

    public MedicineData(string name, decimal rating)
    {
        Name = name;
        Rating = rating;
    }
}