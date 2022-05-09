using SnakeWPF.Model;
using SourceEditor.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media;

namespace SnakeWPF.ViewModel
{
    public class SnakeViewModel : ViewModelBase
    {
        public int Interval { get; set; }
        private int size;
        private bool shouldgo = true;

        CellViewModel[,] buttonGrid;
        ObservableCollection<CellViewModel> buttons;

        public ObservableCollection<CellViewModel> Buttons { get { return buttons; } set { buttons = value; OnPropertyChanged(); } }

        public int Size { get { return size; } set { size = value; OnPropertyChanged(); } }
        public DelegateCommand GameOpenCommand { get; private set; }
        public DelegateCommand GameSaveCommand { get; private set; }
        public DelegateCommand NewGameCommand10 { get; private set; }
        public DelegateCommand NewGameCommand15 { get; private set; }
        public DelegateCommand NewGameCommand20 { get; private set; }
        public DelegateCommand GameExitCommand { get; private set; }
        public DelegateCommand GamePauseCommand { get; private set; }
        public DelegateCommand GameContinueCommand { get; private set; }
        public DelegateCommand UpCommand { get; private set; }
        public DelegateCommand LeftCommand { get; private set; }
        public DelegateCommand DownCommand { get; private set; }
        public DelegateCommand RightCommand { get; private set; }

        public event EventHandler OpenFile;
        public event EventHandler SaveFile;
        public event EventHandler Exit;
        public event EventHandler GameOver;

        public event EventHandler<bool> ChangeTimer;

        private readonly SnakeModel model;
        private Directions step = Directions.UP;

        public SnakeViewModel(SnakeModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            Buttons = new ObservableCollection<CellViewModel>();
            
            this.model = model;

            model.GameOver += new EventHandler(gameOver);
            model.CandyPopUp += new EventHandler(candyDraw);
            model.RemoveLastSnake += new EventHandler(removeLast);
            model.NewGame += new EventHandler(loadGame);

            GameOpenCommand = new DelegateCommand((_) => { OpenFile?.Invoke(this, EventArgs.Empty); candyDraw(this, EventArgs.Empty); });

            GameSaveCommand = new DelegateCommand((_) => SaveFile?.Invoke(this, EventArgs.Empty));

            GameExitCommand = new DelegateCommand((_) => Exit?.Invoke(this, EventArgs.Empty));

            NewGameCommand10 = new DelegateCommand((_) => GameStart(10));
            NewGameCommand15 = new DelegateCommand((_) => GameStart(15));
            NewGameCommand20 = new DelegateCommand((_) => GameStart(20));
            GamePauseCommand = new DelegateCommand((_) => ChangeTimer?.Invoke(this, false));
            GameContinueCommand = new DelegateCommand((_) => ChangeTimer?.Invoke(this, shouldgo));
            UpCommand = new DelegateCommand((_) => step = Directions.UP);
            LeftCommand = new DelegateCommand((_) => step = Directions.LEFT);
            RightCommand = new DelegateCommand((_) => step = Directions.RIGHT);
            DownCommand = new DelegateCommand((_) => step = Directions.DOWN);

        }
        
        private void gameOver(object sender, EventArgs e)
        {
            GameOver?.Invoke(this, EventArgs.Empty);
        }
        
        public void GameStart(Int32 tablesize)
        {
            step = Directions.UP;
            Size = tablesize;
            model.Size = tablesize;
            ChangeTimer?.Invoke(this, false);
            model.Candy = new Point(0, 0);

            CreateGameTable();

            model.firstSnake(tablesize);
            model.getWalls(tablesize);

            DrawSnake();
            DrawWalls();

            ChangeTimer?.Invoke(this, true);
        }

        public void loadGame(object sender, EventArgs e)
        {
            step = model.Direction;
            ChangeTimer?.Invoke(this, false);
            Size = model.Size;

            CreateGameTable();

            model.getWalls(model.Size);

            DrawSnake();
            DrawWalls();
            candyDraw(this, EventArgs.Empty);

            ChangeTimer?.Invoke(this, true);
        }

        public void Model_Tick()
        {
            model.Direction = step;
            model.Seconds += Interval;
            model.shouldCandy();
            model.snakeMove();
            if (!model.isGameOver())
            {
                shouldgo = true;
                DrawSnake();
            } else
            {
                shouldgo = false;
            }
        }

        private void CreateGameTable()
        {

            Int32 size = model.Size;

            buttonGrid = new CellViewModel[size + 2, size + 2];

            buttons.Clear();

            for (int i = 0; i < size + 2; i++)
            {
                for (int j = 0; j < size + 2; j++)
                {
                    buttonGrid[i, j] = new CellViewModel();
                    var tmp = buttonGrid[i, j];
                    tmp.I = i;
                    tmp.J = j;
                    tmp.Color = new SolidColorBrush();
                    tmp.Color.Color = System.Windows.Media.Color.FromRgb(105, 105, 105);
                    Buttons.Add(tmp);
                }
            }

        }

        private void DrawWalls()
        {
            foreach (var elem in model.Walls)
            {
                buttonGrid[elem.X, elem.Y].Color.Color = System.Windows.Media.Color.FromRgb(0, 0, 0);
            }
        }

        private void removeLast(object sender, EventArgs e)
        {
            Point last = model.Snake.Last.Value;
            buttonGrid[last.X, last.Y].Color.Color = System.Windows.Media.Color.FromRgb(105, 105, 105);
        }

        private void DrawSnake()
        {
            foreach (var elem in model.Snake)
            {
                buttonGrid[elem.X, elem.Y].Color.Color = System.Windows.Media.Color.FromRgb(50, 205, 50);
            }
        }

        private void candyDraw(object sender, EventArgs e)
        {
            Point candy = model.Candy;
            if (candy.X == 0 && candy.Y == 0) return;
            buttonGrid[candy.X, candy.Y].Color.Color = System.Windows.Media.Color.FromRgb(255, 20, 20);
        }

    }
}
