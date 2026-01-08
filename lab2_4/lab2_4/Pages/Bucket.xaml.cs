using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using lab2_4.Entity;
using lab2_4.Request;

namespace lab2_4
{
    public partial class Bucket : Window
    {
        private MedicineService _medicineService;
        public ObservableCollection<Item> FilteredItems { get; set; }
        public string Username { get; set; }

        public Bucket()
        {
            InitializeComponent();
            _medicineService = new MedicineService();
            
            // FilteredItems = _medicineService.GetAllMedicines();
            FilteredItems = new ObservableCollection<Item>();
            GetFromServer();
            DataContext = this;
        }

        private async void GetFromServer()
        {
            var items = await GetBucket.Send();
            
            FilteredItems.Clear();
            foreach (var item in items)
            {
                FilteredItems.Add(item);
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            try
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while closing the bucket: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BucketDeleteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var item = button?.DataContext as Item;

                if (item == null) return;

                if (await DeleteFromBucketRequest.Send(item.Id))
                {
                    MessageBox.Show($"Видалено з корзини {item.Name}", "Success");
                    FilteredItems.Remove(item);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Невірний формат номера телефону.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while deleting the item: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ReceiptClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var (success, OrderNumber, Points) = await GetReceiptRequest.Send();
                if (success)
                {
                    var receipt = new Receipt(OrderNumber.ToString(), FilteredItems);
                    receipt.Show();
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while retrieving the receipt: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
