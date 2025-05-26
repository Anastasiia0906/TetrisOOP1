using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tetris
{
    /// <summary>
    /// Представляє тетроміно (фігуру) у грі Тетріс.
    /// </summary>
    public class Tetromino
    {
        private int[,] shape; // Матриця форми фігури (1 — зайнята клітинка, 0 — пуста)

        public int[,] Shape => shape; // Публічний доступ до форми

        public Brush Color { get; }   // Колір фігури

        public int X { get; set; }    // Позиція фігури по горизонталі на полі
        public int Y { get; set; }    // Позиція фігури по вертикалі на полі

        public int Id { get; }        // Ідентифікатор типу фігури

        /// <summary>
        /// Конструктор тетроміно.
        /// </summary>
        public Tetromino(int[,] shape, Brush color, int id)
        {
            this.shape = shape;
            this.Color = color;
            this.Id = id;
            this.X = 0;
            this.Y = 0;
        }

        /// <summary>
        /// Ітерація по зайнятих клітинках фігури.
        /// </summary>
        public static IEnumerable<(int x, int y)> IterateCells(int[,] shape)
        {
            for (int r = 0; r < shape.GetLength(0); r++)
                for (int c = 0; c < shape.GetLength(1); c++)
                    if (shape[r, c] != 0)
                        yield return (c, r); // координати (x, y)
        }

        /// <summary>
        /// Ітерація по клітинках з урахуванням зсуву.
        /// </summary>
        public static IEnumerable<(int x, int y)> IterateCells(int[,] shape, int offsetX, int offsetY)
        {
            for (int r = 0; r < shape.GetLength(0); r++)
                for (int c = 0; c < shape.GetLength(1); c++)
                    if (shape[r, c] != 0)
                        yield return (c + offsetX, r + offsetY);
        }

        /// <summary>
        /// Повертає новий тетроміно, обернутий на 90 градусів.
        /// </summary>
        public Tetromino GetRotated()
        {
            int rows = shape.GetLength(0);
            int cols = shape.GetLength(1);
            int[,] rotated = new int[cols, rows];

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    rotated[c, rows - 1 - r] = shape[r, c]; // Обертання за годинниковою стрілкою

            return new Tetromino(rotated, this.Color, this.Id)
            {
                X = this.X,
                Y = this.Y
            };
        }

        /// <summary>
        /// Обертає фігуру inplace.
        /// </summary>
        public void Rotate()
        {
            shape = GetRotated().shape;
        }

        /// <summary>
        /// Клонує поточну фігуру.
        /// </summary>
        public Tetromino Clone()
        {
            int rows = shape.GetLength(0);
            int cols = shape.GetLength(1);
            int[,] shapeCopy = new int[rows, cols];

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    shapeCopy[r, c] = shape[r, c];

            return new Tetromino(shapeCopy, this.Color, this.Id)
            {
                X = this.X,
                Y = this.Y
            };
        }

        /// <summary>
        /// Малює фігуру на вказаному Canvas.
        /// </summary>
        public void Draw(Canvas canvas, int cellSize)
        {
            foreach (var (x, y) in IterateCells(shape))
            {
                var rect = new Rectangle
                {
                    Width = cellSize,
                    Height = cellSize,
                    Fill = Color,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(rect, (X + x) * cellSize);
                Canvas.SetTop(rect, (Y + y) * cellSize);
                canvas.Children.Add(rect);
            }
        }

        /// <summary>
        /// Малює "тінь" фігури на Canvas (позицію, де вона приземлиться).
        /// </summary>
        public void DrawGhost(Canvas canvas, int cellSize)
        {
            SolidColorBrush ghostBrush = new SolidColorBrush(((SolidColorBrush)Color).Color)
            {
                Opacity = 0.3 // Прозорість для тіні
            };

            foreach (var (x, y) in IterateCells(shape))
            {
                var rect = new Rectangle
                {
                    Width = cellSize,
                    Height = cellSize,
                    Fill = ghostBrush,
                    Stroke = Brushes.Transparent
                };
                Canvas.SetLeft(rect, (X + x) * cellSize);
                Canvas.SetTop(rect, (Y + y) * cellSize);
                canvas.Children.Add(rect);
            }
        }
    }
}
