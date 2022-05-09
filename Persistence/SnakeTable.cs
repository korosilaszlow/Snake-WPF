using SnakeWPF.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SnakeWPF.Persistence
{
    public class SnakeTable
    {
        public Directions direction = Directions.UP;
        public readonly Point nullcandy = new Point(0, 0);

        public Point Candy { get; set; } = new Point(0, 0);
        public List<Point> Walls { get; private set; } = new List<Point>();
        public Int32 Size { get; set; } = 10;
        public Int32 Seconds { get; set; } = 0;

        public LinkedList<Point> Snake { get; private set; } = new LinkedList<Point>();

        public Directions Direction
        {
            get { return direction; }
            set
            {
                if (direction == Directions.UP && value == Directions.DOWN) return;
                if (direction == Directions.LEFT && value == Directions.RIGHT) return;
                if (direction == Directions.RIGHT && value == Directions.LEFT) return;
                if (direction == Directions.DOWN && value == Directions.UP) return;
                direction = value;
            }
        }
        public SnakeTable()
        {

        }

        public SnakeTable(int sec, int size, Directions dir, Point candy, List<Point> walls, List<Point> snake)
        {
            Seconds = sec;
            Size = size;
            direction = dir;
            Candy = candy;
            Walls = walls;
            Snake.Clear();
            foreach (var elem in snake)
            {
                Snake.AddLast(elem);
            }
        }
    }
}
