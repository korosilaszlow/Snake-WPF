using SnakeWPF.Persistence;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace SnakeWPF.Model
{
    public class SnakeModel
    {

        public Directions Direction { get { return table.Direction; } set { table.Direction = value; } }
        public Point Candy { get { return table.Candy; } set { table.Candy = value; } }
        public Int32 Seconds { get { return table.Seconds; } set { table.Seconds = value; } }
        public Int32 Size { get { return table.Size; } set { table.Size = value; } }
        public LinkedList<Point> Snake { get { return table.Snake; } }
        public List<Point> Walls { get { return table.Walls; } }

        public SnakeTable Table { get { return table; } }

        public event EventHandler GameOver;
        public event EventHandler CandyPopUp;
        public event EventHandler RemoveLastSnake;
        public event EventHandler NewGame;

        private readonly Random rand = new Random();

        private readonly ISnakeDataAccess persistence;
        private SnakeTable table;

        public SnakeModel()
        {
            this.persistence = new SnakeDataAccess();
            this.table = new SnakeTable();
        }

        public SnakeModel(ISnakeDataAccess persistence, SnakeTable table)
        {
            if (persistence == null)
            {
                throw new ArgumentNullException(nameof(persistence));
            }

            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            this.persistence = persistence;
            this.table = table;
        }

        public async Task OpenAsync(string path)
        {
            if (persistence == null)
                throw new InvalidOperationException("No data access is provided.");

            table = await persistence.LoadAsync(path);
            OnNewGame();
        }


        public async Task SaveAsync(string path, SnakeTable table)
        {
            if (persistence == null)
                throw new InvalidOperationException("No data access is provided.");

            await persistence.SaveAsync(path, table);
        }

        public void firstSnake(Int32 n)
        {
            table.direction = Directions.UP;
            table.Snake.Clear();
            for (Int32 i = 0; i < 5; ++i)
            {
                table.Snake.AddFirst(new Point((n + 2) / 2 - i, (n + 2) / 2));
            }
        }

        public void firstSnake(List<Point> snake, Directions dir)
        {
            table.direction = dir;
            table.Snake.Clear();
            foreach (var elem in snake)
            {
                table.Snake.AddLast(elem);
            }
        }

        public void getWalls(Int32 n)
        {
            table.Walls.Clear();
            switch (n)
            {
                case 10:
                    table.Walls.Add(new Point(1, 7));
                    table.Walls.Add(new Point(2, 4));
                    table.Walls.Add(new Point(2, 9));
                    table.Walls.Add(new Point(6, 7));
                    table.Walls.Add(new Point(5, 7));
                    break;
                case 15:
                    table.Walls.Add(new Point(1, 13));
                    table.Walls.Add(new Point(4, 14));
                    table.Walls.Add(new Point(3, 14));
                    table.Walls.Add(new Point(6, 7));
                    table.Walls.Add(new Point(6, 14));
                    table.Walls.Add(new Point(4, 9));
                    table.Walls.Add(new Point(14, 11));
                    table.Walls.Add(new Point(2, 11));
                    table.Walls.Add(new Point(8, 12));
                    table.Walls.Add(new Point(8, 15));
                    table.Walls.Add(new Point(10, 15));
                    table.Walls.Add(new Point(7, 13));
                    break;
                case 20:
                    table.Walls.Add(new Point(11, 19));
                    table.Walls.Add(new Point(3, 10));
                    table.Walls.Add(new Point(2, 12));
                    table.Walls.Add(new Point(1, 12));
                    table.Walls.Add(new Point(4, 12));
                    table.Walls.Add(new Point(4, 18));
                    table.Walls.Add(new Point(7, 12));
                    table.Walls.Add(new Point(17, 20));
                    table.Walls.Add(new Point(10, 3));
                    table.Walls.Add(new Point(2, 16));
                    table.Walls.Add(new Point(8, 20));
                    table.Walls.Add(new Point(2, 8));
                    table.Walls.Add(new Point(15, 19));
                    table.Walls.Add(new Point(13, 20));
                    table.Walls.Add(new Point(11, 15));
                    table.Walls.Add(new Point(5, 7));
                    table.Walls.Add(new Point(3, 8));
                    table.Walls.Add(new Point(13, 14));
                    table.Walls.Add(new Point(1, 13));
                    table.Walls.Add(new Point(12, 17));
                    table.Walls.Add(new Point(15, 16));
                    table.Walls.Add(new Point(11, 16));
                    table.Walls.Add(new Point(7, 13));
                    break;
            }
            for (Int32 i = 0; i < n + 2; ++i)
            {
                table.Walls.Add(new Point(0, i));
                table.Walls.Add(new Point(i, 0));
                table.Walls.Add(new Point(n + 1, i));
                table.Walls.Add(new Point(i, n + 1));
            }
        }

        private void OnRemoveLastSnake()
        {
                RemoveLastSnake?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameOver()
        {
                GameOver?.Invoke(this, EventArgs.Empty);
        }

        private void OnCandyPopUp()
        {
                CandyPopUp?.Invoke(this, EventArgs.Empty);
        }

        private void OnNewGame()
        {
                NewGame?.Invoke(this, EventArgs.Empty);
        }

        public void snakeMove()
        {
            Point first = table.Snake.First.Value;
            switch (table.Direction)
            {
                case Directions.UP:
                    table.Snake.AddFirst(new Point(first.X - 1, first.Y));
                    break;
                case Directions.LEFT:
                    table.Snake.AddFirst(new Point(first.X, first.Y - 1));
                    break;
                case Directions.RIGHT:
                    table.Snake.AddFirst(new Point(first.X, first.Y + 1));
                    break;
                case Directions.DOWN:
                    table.Snake.AddFirst(new Point(first.X + 1, first.Y));
                    break;
            }
            if (first == table.Candy)
            {
                table.Candy = new Point(0, 0);
            }
            else
            {
                OnRemoveLastSnake();

                table.Snake.RemoveLast();
            }
        }

        public bool isGameOver()
        {
            Point head = table.Snake.First.Value;
            int count = 0;
            foreach (var elem in table.Snake)
            {
                if (head == elem)
                {
                    ++count;
                }
            }

            if (count > 1)
            {
                OnGameOver();
                return true;
            }

            foreach (var elem in table.Walls)
            {
                if (head == elem)
                {
                    OnGameOver();
                    return true;
                }
            }

            return false;
        }

        public bool shouldCandy()
        {
            bool should = table.Candy == table.nullcandy && (table.Seconds / 1000) % 4 == 0 && table.Seconds > 0;

            if (should)
            {
                int x = 0;
                int y = 0;
                do
                {
                    x = rand.Next(1, table.Size + 1);
                    y = rand.Next(1, table.Size + 1);
                } while (table.Walls.Contains(new Point(x, y)) || table.Snake.Contains(new Point(x, y)));
                table.Candy = new Point(x, y);
                OnCandyPopUp();

            }
            return should;
        }


    }
}
