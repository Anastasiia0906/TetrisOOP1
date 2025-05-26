using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Tetris
{
    public partial class MainWindow : Window, IThemableWindow
    {
        private GameSettings settings;
        private List<Rectangle> animatedBlocks = new List<Rectangle>();
        private Random rand = new Random();

        public MainWindow()
        {
            InitializeComponent();

            // Завантажуємо налаштування гри (можна замінити на App.GlobalSettings, якщо є)
            settings = App.GlobalSettings ?? GameSettings.Load();

            // Запускаємо музику при створенні вікна з передачею налаштувань
            AudioManager.PlayBackgroundMusic(settings);

            // Встановлюємо гучність та стан звуку відповідно до налаштувань
            AudioManager.UpdateVolume(settings);

            Loaded += MainWindow_Loaded;

            ApplyTheme(settings.IsDarkTheme);
        }

        public void ApplyTheme(bool isDarkTheme)
        {
            if (isDarkTheme)
            {
                Background = new SolidColorBrush(Color.FromRgb(37, 37, 38));
                ThemeButton.Content = "СВІТЛА ТЕМА";
                TitleTextBlock.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                Background = Brushes.White;
                ThemeButton.Content = "ТЕМНА ТЕМА";
                TitleTextBlock.Foreground = (Brush)FindResource("TitleGradient");
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeBackgroundAnimation();
        }

        private void InitializeBackgroundAnimation()
        {
            Color[] colors = {
                Colors.Cyan, Colors.Blue, Colors.Orange,
                Colors.Yellow, Colors.Green, Colors.Purple, Colors.Red
            };

            BackgroundCanvas.Children.Clear();
            animatedBlocks.Clear();

            for (int i = 0; i < 25; i++)
            {
                Rectangle block = new Rectangle
                {
                    Width = 35,
                    Height = 35,
                    Opacity = 0.7,
                    Fill = new SolidColorBrush(colors[rand.Next(colors.Length)]),
                    RenderTransform = new TransformGroup()
                };

                // Якщо ActualWidth / Height ще не визначені, встановимо 0 — це безпечно
                double maxWidth = BackgroundCanvas.ActualWidth > 0 ? BackgroundCanvas.ActualWidth : 300;
                double maxHeight = BackgroundCanvas.ActualHeight > 0 ? BackgroundCanvas.ActualHeight : 300;

                Canvas.SetLeft(block, rand.Next(0, (int)maxWidth));
                Canvas.SetTop(block, rand.Next(0, (int)maxHeight));
                BackgroundCanvas.Children.Add(block);
                animatedBlocks.Add(block);

                StartBlockAnimation(block);
            }
        }

        private void StartBlockAnimation(Rectangle block)
        {
            double duration = 5 + rand.NextDouble() * 10;

            double maxWidth = BackgroundCanvas.ActualWidth > 0 ? BackgroundCanvas.ActualWidth : 300;
            double maxHeight = BackgroundCanvas.ActualHeight > 0 ? BackgroundCanvas.ActualHeight : 300;

            DoubleAnimation xAnim = new DoubleAnimation
            {
                From = Canvas.GetLeft(block),
                To = rand.Next(0, (int)maxWidth),
                Duration = TimeSpan.FromSeconds(duration),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            DoubleAnimation yAnim = new DoubleAnimation
            {
                From = Canvas.GetTop(block),
                To = rand.Next(0, (int)maxHeight),
                Duration = TimeSpan.FromSeconds(duration * 1.3),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            block.BeginAnimation(Canvas.LeftProperty, xAnim);
            block.BeginAnimation(Canvas.TopProperty, yAnim);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var gameWindow = new GameWindow(settings, this);
            gameWindow.Owner = this;
            gameWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            gameWindow.Show();
            this.Hide();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(settings);
            settingsWindow.Owner = this;
            settingsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (settingsWindow.ShowDialog() == true)
            {
                // Оновлюємо тему та гучність, якщо змінилися налаштування
                ApplyTheme(settings.IsDarkTheme);
                AudioManager.UpdateVolume(settings);
            }
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            // Змінюємо тему в глобальних налаштуваннях
            settings.IsDarkTheme = !settings.IsDarkTheme;
            App.ApplyTheme(settings.IsDarkTheme);
            ApplyTheme(settings.IsDarkTheme);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (IsLoaded && BackgroundCanvas.ActualWidth > 0)
            {
                InitializeBackgroundAnimation();
            }
        }
    }
}
