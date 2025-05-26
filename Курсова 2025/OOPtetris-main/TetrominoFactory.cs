using System;
using System.Windows.Media;

namespace Tetris
{
    // Статичний клас-фабрика для створення випадкових фігур тетрісу
    public static class TetrominoFactory
    {
        // Генератор випадкових чисел
        private static readonly Random random = new();

        // Масив усіх можливих фігур з відповідним кольором
        private static readonly (int[,] Shape, Brush Color)[] Shapes = new (int[,], Brush)[]
        {
            (new int[,] { {1,1,1,1} }, Brushes.Cyan),        // I-фігура
            (new int[,] { {1,1}, {1,1} }, Brushes.Yellow),   // O-фігура
            (new int[,] { {0,1,0}, {1,1,1} }, Brushes.Purple), // T-фігура
            (new int[,] { {0,1,1}, {1,1,0} }, Brushes.Green),  // S-фігура
            (new int[,] { {1,1,0}, {0,1,1} }, Brushes.Red),    // Z-фігура
            (new int[,] { {1,0,0}, {1,1,1} }, Brushes.Blue),   // J-фігура
            (new int[,] { {0,0,1}, {1,1,1} }, Brushes.Orange)  // L-фігура
        };

        // Створення випадкової фігури
        public static Tetromino CreateRandom()
        {
            int index = random.Next(Shapes.Length);      // Випадковий індекс фігури
            var (shape, color) = Shapes[index];          // Отримання форми та кольору
            return new Tetromino(shape, color, index + 1); // Створення Tetromino (Id = index + 1 для відповідності палеті)
        }
    }
}
