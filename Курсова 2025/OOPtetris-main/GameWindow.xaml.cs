using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tetris
{
    public partial class GameWindow : Window, IThemableWindow
    {
        private GameBoard board;
        private readonly GameSettings settings;
        private readonly MainWindow mainWindow;
        private DispatcherTimer gameTimer;
        private int score;
        private bool isPaused;

        public GameWindow(GameSettings settings, MainWindow mainWindow)
        {
            InitializeComponent();
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
            InitializeGame();
            ApplyTheme(App.IsDarkTheme);
        }

        public void ApplyTheme(bool isDarkTheme)
        {
            if (isDarkTheme)
            {
                MainBorder.Background = new SolidColorBrush(Color.FromRgb(37, 37, 38));
                MainBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(90, 90, 90));
                ScoreText.Foreground = Brushes.White;
            }
            else
            {
                MainBorder.Background = Brushes.White;
                MainBorder.BorderBrush = Brushes.Gray;
                ScoreText.Foreground = Brushes.Black;
            }
        }

        private void InitializeGame()
        {
            if (board != null)
            {
                board.StateChanged -= UpdateUI;
                board.LinesCleared -= OnLinesCleared;
                board.GameOver -= OnGameOver;
            }

            if (gameTimer != null)
            {
                gameTimer.Stop();
            }

            // Виправлення: передаємо settings у конструктор GameBoard
            board = new GameBoard(settings);
            score = 0;
            isPaused = false;
            ScoreText.Text = "0";

            board.StateChanged += UpdateUI;
            board.LinesCleared += OnLinesCleared;
            board.GameOver += OnGameOver;

            gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000 / settings.GameSpeed)
            };
            gameTimer.Tick += (s, e) => board.Tick();
            gameTimer.Start();

            UpdateUI();
        }

        private void UpdateUI()
        {
            GameRenderer.Render(GameCanvas, board, 25);
            RenderNextPiece();
        }

        private void RenderNextPiece()
        {
            NextPieceCanvas.Children.Clear();
            if (board.NextPiece == null) return;

            foreach (var (x, y) in Tetromino.IterateCells(board.NextPiece.Shape))
            {
                var rect = new Rectangle
                {
                    Width = 25,
                    Height = 25,
                    Fill = board.NextPiece.Color,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(rect, x * 25 + 25);
                Canvas.SetTop(rect, y * 25 + 25);
                NextPieceCanvas.Children.Add(rect);
            }
        }

        private void OnLinesCleared(int lines)
        {
            score += lines * 100;
            ScoreText.Text = score.ToString();
        }

        private void OnGameOver()
        {
            gameTimer.Stop();
            MessageBox.Show($"Гра завершена! Ваш рахунок: {score}", "Кінець гри");
            ExitToMainMenu();
        }

        private void ExitToMainMenu()
        {
            mainWindow.Show();
            this.Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (isPaused) return;

            switch (e.Key)
            {
                case Key key when key == settings.LeftKey:
                    board.MoveLeft();
                    break;
                case Key key when key == settings.RightKey:
                    board.MoveRight();
                    break;
                case Key key when key == settings.RotateKey:
                    board.Rotate();
                    break;
                case Key key when key == settings.DownKey:
                    board.MoveDown();
                    break;
                case Key key when key == settings.DropKey:
                    board.Drop();
                    break;
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            isPaused = !isPaused;
            PauseButton.Content = isPaused ? "ПРОДОВЖИТИ" : "ПАУЗА";
            if (isPaused) gameTimer.Stop();
            else gameTimer.Start();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(settings)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (settingsWindow.ShowDialog() == true)
            {
                gameTimer.Interval = TimeSpan.FromMilliseconds(1000 / settings.GameSpeed);
                ApplyTheme(App.IsDarkTheme);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ExitToMainMenu();
        }
    }
}
