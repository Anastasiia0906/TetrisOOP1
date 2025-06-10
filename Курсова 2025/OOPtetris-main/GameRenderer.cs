using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tetris
{
    /// Рендерить ігрове поле на Canvas: сітка, фігури, тінь.
    public static class GameRenderer
    {
        // Кольори: [0] — фон, далі — фігури
        private static readonly Brush[] _palette =
        {
            Brushes.Transparent, Brushes.Cyan,   Brushes.Blue,
            Brushes.Orange,      Brushes.Yellow, Brushes.Green,
            Brushes.Purple,      Brushes.Red
        };

        /// Малює поле, фіксовані блоки, активну фігуру і її тінь.
        public static void Render(Canvas canvas, GameBoard board, int cellSize)
        {
            canvas.Children.Clear();

            // Сітка
            DrawGrid(canvas, board, cellSize);

            // Зафіксовані блоки
            foreach (var (x, y, id) in board.IterateFixedBlocks())
                DrawRect(canvas, x, y, cellSize, _palette[id]);

            // Поточна фігура
            if (board.ActivePiece != null)
            {
                foreach (var (cx, cy) in Tetromino.IterateCells(board.ActivePiece.Shape))
                    DrawRect(canvas,
                             board.ActivePiece.X + cx,
                             board.ActivePiece.Y + cy,
                             cellSize,
                             _palette[board.ActivePiece.Id]);
            }

            // Тінь фігури
            var (ghostX, ghostY) = board.GetGhostPosition();
            if (board.ActivePiece != null)
            {
                foreach (var (cx, cy) in Tetromino.IterateCells(board.ActivePiece.Shape))
                    DrawRect(canvas,
                             ghostX + cx,
                             ghostY + cy,
                             cellSize,
                             new SolidColorBrush(Color.FromArgb(40, 0, 0, 0))); // прозорий чорний
            }
        }

        /// Малює сітку по всьому полю.
        private static void DrawGrid(Canvas canvas, GameBoard board, int cellSize)
        {
            var gridBrush = Brushes.White;
            double opacity = 0.07;
            double thickness = 0.5;

            int width = GameBoard.Cols * cellSize;
            int height = GameBoard.Rows * cellSize;

            // Вертикальні
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

            // Горизонтальні
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

          /// Малює одну клітинку.
             private static void DrawRect(Canvas cv, int x, int y, int cell, Brush fill)
        {
            var r = new Rectangle
            {
                Width = cell,
                Height = cell,
                Fill = fill,
                Stroke = Brushes.Black, // рамка
                StrokeThickness = 1
            };

            Canvas.SetLeft(r, x * cell);
            Canvas.SetTop(r, y * cell);
            cv.Children.Add(r);
        }
    }
}
