using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tetris
{
    // Представляє одну фігуру в грі
    public class Tetromino
    {
        private int[,] shape; // 1 — клітинка зайнята, 0 — пуста

        public int[,] Shape => shape; // доступ до форми фігури
        public Brush Color { get; }   // колір фігури

        public int X { get; set; }    // позиція по горизонталі
        public int Y { get; set; }    // позиція по вертикалі

        public int Id { get; }        // тип фігури (0–6)

        public Tetromino(int[,] shape, Brush color, int id)
        {
            this.shape = shape;
            this.Color = color;
            this.Id = id;
            this.X = 0;
            this.Y = 0;
        }

        // Прохід по всіх зайнятих клітинках фігури
        public static IEnumerable<(int x, int y)> IterateCells(int[,] shape)
        {
            for (int r = 0; r < shape.GetLength(0); r++)
                for (int c = 0; c < shape.GetLength(1); c++)
                    if (shape[r, c] != 0)
                        yield return (c, r);
        }

        // Те ж саме, але зі зсувом
        public static IEnumerable<(int x, int y)> IterateCells(int[,] shape, int offsetX, int offsetY)
        {
            for (int r = 0; r < shape.GetLength(0); r++)
                for (int c = 0; c < shape.GetLength(1); c++)
                    if (shape[r, c] != 0)
                        yield return (c + offsetX, r + offsetY);
        }

        // Повертає нову фігуру, обернену на 90 градусів
        public Tetromino GetRotated()
        {
            int rows = shape.GetLength(0);
            int cols = shape.GetLength(1);
            int[,] rotated = new int[cols, rows];

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    rotated[c, rows - 1 - r] = shape[r, c];

            return new Tetromino(rotated, this.Color, this.Id)
            {
                X = this.X,
                Y = this.Y
            };
        }

        // Обертає фігуру (міняє форму)
        public void Rotate()
        {
            shape = GetRotated().shape;
        }

        // Клонує фігуру з усіма параметрами
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

        // Малює фігуру на Canvas
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

        // Малює "тінь" фігури — де вона впаде
        public void DrawGhost(Canvas canvas, int cellSize)
        {
            SolidColorBrush ghostBrush = new SolidColorBrush(((SolidColorBrush)Color).Color)
            {
                Opacity = 0.3
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
