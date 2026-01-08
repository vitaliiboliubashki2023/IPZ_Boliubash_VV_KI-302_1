using System.ComponentModel;
using System.Text.RegularExpressions;

namespace lab2_4.Entity
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        private string _username;

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value?.Trim(); // Обрізаємо пробіли на початку та в кінці
                    OnPropertyChanged(nameof(Username));
                    OnPropertyChanged(nameof(IsValid)); // Оновити стан валідації
                }
            }
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(Username) && Regex.IsMatch(Username, @"^\d{10}$"); // Перевірка формату номера телефону

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}