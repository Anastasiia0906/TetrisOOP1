using System.Windows;

namespace Tetris
{
    public partial class App : Application
    {
        public static GameSettings GlobalSettings { get; private set; }
        public static bool IsDarkTheme { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Завантажуємо глобальні налаштування (реалізація GameSettings.Load() потрібна)
            GlobalSettings = GameSettings.Load() ?? new GameSettings();

            // Отримуємо поточну тему з налаштувань
            IsDarkTheme = GlobalSettings.IsDarkTheme;

            // Застосовуємо тему до всіх вікон (включно з першим)
            ApplyTheme(IsDarkTheme);
        }

        public static void ApplyTheme(bool isDark)
        {
            IsDarkTheme = isDark;
            GlobalSettings.IsDarkTheme = isDark;

            // Зберігаємо налаштування
            GlobalSettings.Save();

            // Оновлюємо тему у всіх відкритих вікнах, які реалізують IThemableWindow
            if (Current is App app)
            {
                foreach (Window w in app.Windows)
                {
                    if (w is IThemableWindow themableWindow)
                    {
                        themableWindow.ApplyTheme(isDark);
                    }
                }
            }
        }
    }
}
