using System.Windows;

namespace Tetris
{
    // Клас SettingsWindow відповідає за відображення та обробку вікна налаштувань гри
    public partial class SettingsWindow : Window
    {
        // Властивість для збереження поточних налаштувань гри
        public GameSettings Settings { get; }

        // Конструктор вікна, приймає об’єкт налаштувань
        public SettingsWindow(GameSettings settings)
        {
            InitializeComponent(); // Ініціалізація компонентів інтерфейсу
            Settings = settings ?? new GameSettings(); // Якщо налаштування не передано — створюється новий об’єкт
            LoadSettings(); // Завантаження налаштувань у UI
        }

        // Метод завантажує значення налаштувань у відповідні елементи інтерфейсу
        private void LoadSettings()
        {
            GameSpeedSlider.Value = Settings.GameSpeed; // Встановлення значення швидкості гри

            VolumeSlider.Value = Settings.Volume; // Встановлення рівня гучності
            VolumeSlider.IsEnabled = Settings.SoundEnabled; // Увімкнення/вимкнення повзунка залежно від стану звуку

            // Відображення поточного стану звуку на кнопці
            SoundToggleButton.Content = Settings.SoundEnabled ? "Звук: Увімкнено" : "Звук: Вимкнено";

            // Відображення поточної теми інтерфейсу
            ThemeToggleButton.Content = Settings.IsDarkTheme ? "Темна тема" : "Світла тема";
        }

        // Обробник натискання кнопки перемикання звуку
        private void SoundToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.SoundEnabled = !Settings.SoundEnabled; // Зміна стану звуку
            VolumeSlider.IsEnabled = Settings.SoundEnabled; // Активність повзунка залежить від стану
            SoundToggleButton.Content = Settings.SoundEnabled ? "Звук: Увімкнено" : "Звук: Вимкнено"; // Оновлення тексту кнопки
        }

        // Обробник натискання кнопки перемикання теми
        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.IsDarkTheme = !Settings.IsDarkTheme; // Зміна теми
            App.ApplyTheme(Settings.IsDarkTheme); // Застосування теми до всієї програми
            ThemeToggleButton.Content = Settings.IsDarkTheme ? "Темна тема" : "Світла тема"; // Оновлення тексту кнопки
        }

        // Обробник натискання кнопки "Зберегти"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.GameSpeed = (int)GameSpeedSlider.Value; // Збереження нової швидкості гри
            Settings.Volume = (int)VolumeSlider.Value; // Збереження нового рівня гучності
            Settings.Save(); // Виклик методу збереження налаштувань (наприклад, у файл)

            DialogResult = true; // Повернення позитивного результату діалогу
            Close(); // Закриття вікна
        }

        // Обробник натискання кнопки "Скасувати"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Відміна змін — повертає false
            Close(); // Закриття вікна без збереження
        }
    }
}
