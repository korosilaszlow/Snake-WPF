using SnakeWPF.Model;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace SnakeWPF.Persistence
{
    public class SnakeDataAccess : ISnakeDataAccess
    {
        public async Task<SnakeTable> LoadAsync(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    String line = await reader.ReadLineAsync();
                    String[] numbers = line.Split(' ');
                    SnakeTable table = new SnakeTable();

                    table.Size = Int32.Parse(numbers[0]);
                    table.Candy = new Point(Int32.Parse(numbers[1]), Int32.Parse(numbers[2]));

                    switch (Int32.Parse(numbers[3]))
                    {
                        case 0:
                            table.direction = Directions.UP;
                            table.Direction = Directions.UP;
                            break;
                        case 1:
                            table.direction = Directions.LEFT;
                            table.Direction = Directions.LEFT;
                            break;
                        case 2:
                            table.direction = Directions.RIGHT;
                            table.Direction = Directions.RIGHT;
                            break;
                        case 3:
                            table.direction = Directions.DOWN;
                            table.Direction = Directions.DOWN;
                            break;
                    }

                    table.Seconds = Int32.Parse(numbers[4]);

                    table.Snake.Clear();
                    for (Int32 i = 5; i < numbers.Length; i += 2)
                    {
                        table.Snake.AddLast(new Point(Int32.Parse(numbers[i]), Int32.Parse(numbers[i + 1])));
                    }

                    return table;

                }
            }
            catch
            {
                throw new SnakeDataException();
            }
        }

        public async Task SaveAsync(String path, SnakeTable table)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path)) // fájl megnyitása
                {
                    writer.Write(table.Size); // kiírjuk a méreteket
                    Int32 dir = 0;
                    switch (table.Direction)
                    {
                        case Directions.UP:
                            dir = 0;
                            break;
                        case Directions.LEFT:
                            dir = 1;
                            break;
                        case Directions.RIGHT:
                            dir = 2;
                            break;
                        case Directions.DOWN:
                            dir = 3;
                            break;
                    }
                    await writer.WriteAsync(" " + table.Candy.X + " " + table.Candy.Y + " " + dir + " " + table.Seconds);
                    foreach (var elem in table.Snake)
                    {
                        await writer.WriteAsync(" " + elem.X + " " + elem.Y);
                    }


                }
            }
            catch
            {
                throw new SnakeDataException();
            }
        }
    }
}
