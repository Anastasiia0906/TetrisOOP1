using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tetris
{
    /// <summary>
    /// Клас для візуалізації ігрового поля Tetris на WPF Canvas.
    /// Містить методи для відображення сітки, зафіксованих блоків, активної фігури та "тіні" фігури.
    /// </summary>
    public static class GameRenderer
    {
        // Палітра кольорів: кожному ID фігури відповідає свій колір.
        // Індекс 0 — прозорий (фон), далі йдуть кольори фігур.
        private static readonly Brush[] _palette =
        {
            Brushes.Transparent, Brushes.Cyan,   Brushes.Blue,
            Brushes.Orange,      Brushes.Yellow, Brushes.Green,
            Brushes.Purple,      Brushes.Red
        };

        /// <summary>
        /// Основний метод візуалізації гри.
        /// Малює сітку, фіксовані блоки, активну фігуру та її тінь.
        /// </summary>
        /// <param name="canvas">Полотно для малювання</param>
        /// <param name="board">Об'єкт ігрового поля</param>
        /// <param name="cellSize">Розмір однієї клітинки в пікселях</param>
        public static void Render(Canvas canvas, GameBoard board, int cellSize)
        {
            // Очистити попередній вміст канвасу
            canvas.Children.Clear();

            // 1) Намалювати сітку
            DrawGrid(canvas, board, cellSize);

            // 2) Намалювати всі фіксовані блоки на полі
            foreach (var (x, y, id) in board.IterateFixedBlocks())
                DrawRect(canvas, x, y, cellSize, _palette[id]);

            // 3) Намалювати активну фігуру (ту, що зараз падає)
            if (board.ActivePiece != null)
            {
                foreach (var (cx, cy) in Tetromino.IterateCells(board.ActivePiece.Shape))
                    DrawRect(canvas,
                             board.ActivePiece.X + cx,
                             board.ActivePiece.Y + cy,
                             cellSize,
                             _palette[board.ActivePiece.Id]);
            }

            // 4) Намалювати тінь активної фігури (позиція, куди вона впаде)
            var (ghostX, ghostY) = board.GetGhostPosition();
            if (board.ActivePiece != null)
            {
                foreach (var (cx, cy) in Tetromino.IterateCells(board.ActivePiece.Shape))
                    DrawRect(canvas,
                             ghostX + cx,
                             ghostY + cy,
                             cellSize,
                             new SolidColorBrush(Color.FromArgb(40, 0, 0, 0))); // прозорий чорний для тіні
            }
        }

        /// <summary>
        /// Малює сітку на полі для візуального розділення клітинок.
        /// </summary>
        private static void DrawGrid(Canvas canvas, GameBoard board, int cellSize)
        {
            var gridBrush = Brushes.White;   // колір сітки
            double opacity = 0.07;           // прозорість сітки
            double thickness = 0.5;          // товщина ліній

            int width = GameBoard.Cols * cellSize;
            int height = GameBoard.Rows * cellSize;

            // Вертикальні лінії
            for (int x = 0; x <= GameBoard.Cols; x++)
            {
                var line = new Line
                {
                    X1 = x * cellSize,
                    Y1 = 0,
                    X2 = x * cellSize,
                    Y2 = height,
                    Stroke = gridBrush,
                    StrokeThickness = thickness,
                    Opacity = opacity
                };
                canvas.Children.Add(line);
            }

            // Горизонтальні лінії
            for (int y = 0; y <= GameBoard.Rows; y++)
            {
                var line = new Line
                {
                    X1 = 0,
                    Y1 = y * cellSize,
                    X2 = width,
                    Y2 = y * cellSize,
                    Stroke = gridBrush,
                    StrokeThickness = thickness,
                    Opacity = opacity
                };
                canvas.Children.Add(line);
            }
        }

        /// <summary>
        /// Малює один прямокутник на полі гри.
        /// </summary>
        /// <param name="cv">Полотно</param>
        /// <param name="x">X-координата в клітинках</param>
        /// <param name="y">Y-координата в клітинках</param>
        /// <param name="cell">Розмір клітинки</param>
        /// <param name="fill">Колір фону</param>
        private static void DrawRect(Canvas cv, int x, int y, int cell, Brush fill)
        {
            var r = new Rectangle
            {
                Width = cell,
                Height = cell,
                Fill = fill,
                Stroke = Brushes.Black,      // рамка клітинки — чорна
                StrokeThickness = 1
            };

            // Позиціонування на канвасі
            Canvas.SetLeft(r, x * cell);
            Canvas.SetTop(r, y * cell);

            // Додати до канвасу
            cv.Children.Add(r);
        }
    }
}
