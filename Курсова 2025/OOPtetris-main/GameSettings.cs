using System;
using System.IO;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Tetris
{
    /// <summary>Налаштування гри: швидкість, тема, музика, звук і гарячі клавіші.</summary>
    public class GameSettings
    {
        private const string SettingsFile = "settings.xml";

        // Загальні параметри
        public int Difficulty { get; set; } = 1;

        public int GameSpeed { get; set; } = 5;

        public bool MusicEnabled { get; set; } = true;   // Музика увімкнена/вимкнена
        public bool SoundEnabled { get; set; } = true;   // Звукові ефекти увімкнені/вимкнені

        public int Volume { get; set; } = 70;

        public bool IsDarkTheme { get; set; } = false;

        // Гарячі клавіші (XmlIgnore, щоб не серіалізувати напряму)
        [XmlIgnore]
        public Key LeftKey { get; set; } = Key.Left;

        [XmlIgnore]
        public Key RightKey { get; set; } = Key.Right;

        [XmlIgnore]
        public Key RotateKey { get; set; } = Key.Up;

        [XmlIgnore]
        public Key DownKey { get; set; } = Key.Down;

        [XmlIgnore]
        public Key DropKey { get; set; } = Key.Space;

        // Обгортки для серіалізації ключів у вигляді рядків

        [XmlElement("LeftKey")]
        public string LeftKeyString
        {
            get => LeftKey.ToString();
            set => LeftKey = ParseKey(value, Key.Left);
        }

        [XmlElement("RightKey")]
        public string RightKeyString
        {
            get => RightKey.ToString();
            set => RightKey = ParseKey(value, Key.Right);
        }

        [XmlElement("RotateKey")]
        public string RotateKeyString
        {
            get => RotateKey.ToString();
            set => RotateKey = ParseKey(value, Key.Up);
        }

        [XmlElement("DownKey")]
        public string DownKeyString
        {
            get => DownKey.ToString();
            set => DownKey = ParseKey(value, Key.Down);
        }

        [XmlElement("DropKey")]
        public string DropKeyString
        {
            get => DropKey.ToString();
            set => DropKey = ParseKey(value, Key.Space);
        }

        /// <summary>Парсить ключ з рядка з безпечною обробкою помилок.</summary>
        private static Key ParseKey(string? keyString, Key defaultKey)
        {
            if (!string.IsNullOrEmpty(keyString) && Enum.TryParse<Key>(keyString, out var key))
            {
                return key;
            }
            return defaultKey;
        }

        /// <summary>Завантажує налаштування з XML-файлу або повертає нові за замовчуванням.</summary>
        public static GameSettings Load()
        {
            if (File.Exists(SettingsFile))
            {
                try
                {
                    using var reader = new StreamReader(SettingsFile);
                    var xml = new XmlSerializer(typeof(GameSettings));
                    return (GameSettings)xml.Deserialize(reader)!;
                }
                catch
                {
                    // Якщо сталася помилка десеріалізації, повертаємо налаштування за замовчуванням
                }
            }
            return new GameSettings();
        }

        /// <summary>Зберігає поточні налаштування у XML-файл.</summary>
        public void Save()
        {
            try
            {
                using var writer = new StreamWriter(SettingsFile);
                var xml = new XmlSerializer(typeof(GameSettings));
                xml.Serialize(writer, this);
            }
            catch
            {
                // Ігноруємо помилки при записі (наприклад, відсутність прав на файл)
            }
        }
    }
}
