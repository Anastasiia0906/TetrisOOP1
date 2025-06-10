using System;
using System.IO;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Tetris
{
    // Клас зберігає налаштування гри: складність, швидкість, звук, тему та керування
    public class GameSettings
    {
        private const string SettingsFile = "settings.xml"; // Назва файлу для збереження налаштувань

        // Рівень складності гри (1 — легкий, 2 — середній, 3 — складний)
        public int Difficulty { get; set; } = 1;

        // Швидкість гри (впливає на інтервал таймера)
        public int GameSpeed { get; set; } = 5;

        // Увімкнення фонової музики
        public bool MusicEnabled { get; set; } = true;

        // Увімкнення звукових ефектів
        public bool SoundEnabled { get; set; } = true;

        // Гучність звуку (0–100)
        public int Volume { get; set; } = 70;

        // Темна тема інтерфейсу
        public bool IsDarkTheme { get; set; } = false;

        // Клавіша для руху фігури вліво
        [XmlIgnore]
        public Key LeftKey { get; set; } = Key.Left;

        // Клавіша для руху фігури вправо
        [XmlIgnore]
        public Key RightKey { get; set; } = Key.Right;

        // Клавіша для обертання фігури
        [XmlIgnore]
        public Key RotateKey { get; set; } = Key.Up;

        // Клавіша для прискореного падіння фігури
        [XmlIgnore]
        public Key DownKey { get; set; } = Key.Down;

        // Клавіша для миттєвого скидання фігури
        [XmlIgnore]
        public Key DropKey { get; set; } = Key.Space;

        // Нижче — обгортки для серіалізації клавіш у вигляді рядків (XML не підтримує тип Key напряму)

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

        // Перетворює рядок у клавішу Key, з резервним значенням на випадок помилки
        private static Key ParseKey(string? keyString, Key defaultKey)
        {
            if (!string.IsNullOrEmpty(keyString) && Enum.TryParse<Key>(keyString, out var key))
            {
                return key;
            }
            return defaultKey;
        }

        // Завантаження налаштувань з XML-файлу. Якщо файл не існує або пошкоджений — повертає налаштування за замовчуванням
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
                    // Якщо сталася помилка при читанні або десеріалізації, повертаємо стандартні налаштування
                }
            }
            return new GameSettings();
        }

        // Зберігає поточні налаштування у XML-файл
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
                // Ігноруємо помилки при записі (наприклад, файл зайнятий або немає доступу)
            }
        }
    }
}
