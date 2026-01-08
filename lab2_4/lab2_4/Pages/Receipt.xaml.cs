using System.Collections.ObjectModel;
using System.Windows;
using lab2_4.Entity;

namespace lab2_4
{
    public partial class Receipt : Window
    {
        public ObservableCollection<Item> SelectedItems { get; set; }
        public ReceiptViewModel viewModel { get; set; }
    
        public Receipt(string orderNumber, ObservableCollection<Item> selectedItems)
        {
            InitializeComponent();
            SelectedItems = selectedItems;

            try
            {
                viewModel = new ReceiptViewModel
                {
                    OrderNumber = orderNumber
                };
                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while initializing the receipt: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("An error occurred while closing the receipt: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}