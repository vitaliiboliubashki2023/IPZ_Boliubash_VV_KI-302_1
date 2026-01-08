using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using lab2_4.Entity;
using lab2_4.Pages;
using lab2_4.Request;

namespace lab2_4;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private MedicineService _medicineService;
    public List<Item> Items { get; set; }
    public ObservableCollection<Item> FilteredItems { get; set; }
    public ObservableCollection<string> Categories { get; set; }

    private string _searchName;

    public string SearchName
    {
        get => _searchName;
        set
        {
            _searchName = value;
            OnPropertyChanged(nameof(SearchName));
            UpdateFilteredItems();
        }
    }

    private string _selectedCategory;

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            OnPropertyChanged(nameof(SelectedCategory));
            UpdateFilteredItems();
        }
    }

    public MainWindow()
    {
        InitializeComponent();

        Items = new List<Item>();
        FilteredItems = new ObservableCollection<Item>();
        Categories = new ObservableCollection<string>();
        SelectedCategory = "Усі";

        GetFromServer();

        DataContext = this;
    }

    private async void GetFromServer()
    {
        var categories = await GetMedicineCategories.Send();

        Categories.Clear();
        foreach (var category in categories)
        {
            Categories.Add(category);
        }

        var items = await MedicineService.Send();
        Items.Clear();
        FilteredItems.Clear();
        foreach (var item in items)
        {
            Items.Add(item);
            FilteredItems.Add(item);
        }
    }

    private void UpdateFilteredItems()
    {
        try
        {
            var filteredItems = MedicineService.FilterMedicines(Items, SearchName, SelectedCategory);

            FilteredItems.Clear();

            foreach (var filteredItem in filteredItems)
            {
                FilteredItems.Add(filteredItem);
            }

            OnPropertyChanged(nameof(FilteredItems));
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred while updating the item list: " + ex.Message, "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void BucketAddClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var button = sender as Button;
            var item = button?.DataContext as Item;

            if (item == null) return;

            if (await PutInBucketRequest.Send(item.Id))
            {
                MessageBox.Show($"Добавлено успішно в корзину {item.Name}", "Success");
            }
            else
            {
                MessageBox.Show($"Не вдалося додати {item.Name} до корзини.", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        catch (FormatException)
        {
            MessageBox.Show("Невірний формат номера телефону.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred while adding the item to the bucket: " + ex.Message, "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BucketClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var bucket = new Bucket();
            bucket.Show();
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred while opening the bucket: " + ex.Message, "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void Close(object sender, RoutedEventArgs e)
    {
        var login = new Login();
        login.Show();
        Close();
    }

    private void RateMedicineClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var button = sender as Button;
            var item = button?.DataContext as Item;
            if (item == null) return;

            var rate = new RateMedicine(item);
            rate.Show();
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred: " + ex.Message, "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}