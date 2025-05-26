using System;
using System.Collections.Generic;

namespace Tetris
{
    public class GameBoard
    {
        // Константи для розмірів ігрового поля
        public const int Rows = 20;  // Кількість рядків ігрового поля
        public const int Cols = 10;  // Кількість стовпців ігрового поля

        // Двовимірний масив для зберігання стану кожної клітинки ігрового поля
        // 0 означає порожню клітинку, інші значення - ID типу блоку
        private readonly int[,] _cells = new int[Rows, Cols];

        // Поточна активна фігура (тетроміно) і наступна фігура
        public Tetromino? ActivePiece { get; private set; }
        public Tetromino? NextPiece { get; private set; }

        // Події для сповіщення про зміни стану гри
        public event Action? StateChanged;       // Викликається при зміні стану гри
        public event Action<int>? LinesCleared;  // Викликається при очищенні ліній (параметр - кількість ліній)
        public event Action? GameOver;           // Викликається при завершенні гри

        private readonly GameSettings settings;   // Налаштування гри

        // Конструктор ігрового поля
        public GameBoard(GameSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            // Створюємо першу наступну фігуру і одразу спавнимо її
            NextPiece = TetrominoFactory.CreateRandom();
            SpawnNextPiece();
        }

        // Методи для руху фігури
        public void MoveLeft() => TryMove(-1, 0);  // Рух вліво
        public void MoveRight() => TryMove(1, 0);  // Рух вправо
        public void MoveDown() => TryMove(0, 1);   // Рух вниз

        public void Rotate() => TryRotate();  // Обертання фігури

        // Швидке скидання фігури вниз (drop)
        public void Drop()
        {
            if (ActivePiece == null) return;

            // Рухаємо фігуру вниз, поки це можливо
            while (CanMove(0, 1, ActivePiece.Shape))
                ActivePiece.Y++;

            LockPiece();  // Фіксуємо фігуру на місці
        }

        // Основний ігровий цикл (викликається через певні інтервали часу)
        public void Tick()
        {
            if (ActivePiece == null) return;

            if (CanMove(0, 1, ActivePiece.Shape))
                ActivePiece.Y++;  // Рухаємо фігуру вниз
            else
                LockPiece();      // Якщо рух неможливий - фіксуємо фігуру

            StateChanged?.Invoke();  // Сповіщуємо про зміну стану
        }

        // Спроба перемістити фігуру
        private bool TryMove(int dx, int dy)
        {
            if (ActivePiece == null) return false;

            if (!CanMove(dx, dy, ActivePiece.Shape)) return false;

            ActivePiece.X += dx;
            ActivePiece.Y += dy;
            StateChanged?.Invoke();
            return true;
        }

        // Спроба обернути фігуру
        private bool TryRotate()
        {
            if (ActivePiece == null) return false;

            var rotatedShape = ActivePiece.GetRotated().Shape;

            if (!CanMove(0, 0, rotatedShape)) return false;

            ActivePiece.Rotate();
            StateChanged?.Invoke();
            return true;
        }

        // Перевірка, чи можливий рух фігури
        private bool CanMove(int dx, int dy, int[,] shape)
        {
            if (ActivePiece == null) return false;

            // Перевіряємо кожну клітинку фігури
            foreach (var (cx, cy) in Tetromino.IterateCells(shape))
            {
                int nx = ActivePiece.X + cx + dx;  // Нова X-координата
                int ny = ActivePiece.Y + cy + dy;  // Нова Y-координата

                // Перевірка виходу за межі поля
                if (nx < 0 || nx >= Cols || ny < 0 || ny >= Rows) return false;
                // Перевірка на наявність інших блоків
                if (_cells[ny, nx] != 0) return false;
            }

            return true;
        }

        // Фіксація фігури на ігровому полі
        private void LockPiece()
        {
            if (ActivePiece == null) return;

            // Додаємо блоки фігури до ігрового поля
            foreach (var (cx, cy) in Tetromino.IterateCells(ActivePiece.Shape))
            {
                int x = ActivePiece.X + cx;
                int y = ActivePiece.Y + cy;

                if (y >= 0 && y < Rows && x >= 0 && x < Cols)
                {
                    _cells[y, x] = ActivePiece.Id;  // Записуємо ID фігури
                }
            }

            // Очищаємо заповнені лінії
            int cleared = ClearLines();

            StateChanged?.Invoke();

            if (cleared > 0)
            {
                LinesCleared?.Invoke(cleared);
                AudioManager.PlayLineClearSound(settings);  // Відтворюємо звук очищення
            }

            SpawnNextPiece();  // Створюємо нову фігуру
        }

        // Очищення заповнених ліній
        private int ClearLines()
        {
            int linesCleared = 0;

            // Перевіряємо рядки знизу вгору
            for (int row = Rows - 1; row >= 0; row--)
            {
                bool full = true;
                // Перевіряємо, чи заповнений рядок
                for (int col = 0; col < Cols; col++)
                {
                    if (_cells[row, col] == 0)
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    RemoveLine(row);  // Видаляємо заповнений рядок
                    linesCleared++;
                    row++; // Перевіряємо цей рядок знову після видалення
                }
            }

            return linesCleared;
        }

        // Видалення конкретного рядка
        private void RemoveLine(int rowToRemove)
        {
            // Очищаємо рядок
            for (int col = 0; col < Cols; col++)
                _cells[rowToRemove, col] = 0;

            // Зсуваємо всі блоки вище видаленого рядка вниз
            for (int row = rowToRemove - 1; row >= 0; row--)
            {
                for (int col = 0; col < Cols; col++)
                {
                    if (_cells[row, col] != 0)
                    {
                        int currentRow = row;
                        // Рухаємо блок вниз, поки є місце
                        while (currentRow + 1 < Rows && _cells[currentRow + 1, col] == 0)
                        {
                            _cells[currentRow + 1, col] = _cells[currentRow, col];
                            _cells[currentRow, col] = 0;
                            currentRow++;
                        }
                    }
                }
            }
        }

        // Створення нової фігури
        private void SpawnNextPiece()
        {
            ActivePiece = NextPiece;
            // Розміщуємо фігуру по центру верхньої частини поля
            ActivePiece.X = Cols / 2 - 2;
            ActivePiece.Y = 0;

            // Генеруємо наступну фігуру
            NextPiece = TetrominoFactory.CreateRandom();

            // Перевіряємо, чи можна розмістити нову фігуру
            if (!CanMove(0, 0, ActivePiece.Shape))
            {
                GameOver?.Invoke();  // Гра закінчена, якщо немає місця
            }
            else
            {
                StateChanged?.Invoke();
            }
        }

        // Ітератор по зафіксованим блокам на полі
        public IEnumerable<(int x, int y, int id)> IterateFixedBlocks()
        {
            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Cols; x++)
                    if (_cells[y, x] != 0)
                        yield return (x, y, _cells[y, x]);
        }

        // Отримання позиції "тіні" фігури (де вона опиниться при drop)
        public (int x, int ghostY) GetGhostPosition()
        {
            if (ActivePiece == null) return (0, 0);

            int originalY = ActivePiece.Y;

            // Рухаємо фігуру вниз, поки це можливо
            while (CanMove(0, 1, ActivePiece.Shape))
                ActivePiece.Y++;

            int ghostY = ActivePiece.Y;
            ActivePiece.Y = originalY;  // Повертаємо фігуру на початкову позицію

            return (ActivePiece.X, ghostY);
        }
    }
}