using Microsoft.Win32;
using SnakeWPF.Model;
using SnakeWPF.Persistence;
using SnakeWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SnakeWPF
{
    
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 

    public partial class App : Application
    {
        //[DllImport("Kernel32")]
        //public static extern void AllocConsole();
        
        private ISnakeDataAccess persistence;
        private SnakeModel model;
        private SnakeViewModel viewmodel;
        private SnakeTable table;
        private DispatcherTimer timer;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;

        public App()
        {
            //AllocConsole();
            MainWindow mainWindow = new MainWindow();

            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Snake game file (*.sgs)|*.sgs";
            openFileDialog.Title = "Snake game load";
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Snake game file (*.sgs)|*.sgs";
            saveFileDialog.Title = "Snake game save";

            persistence = new SnakeDataAccess();
            table = new SnakeTable();
            model = new SnakeModel(persistence, table);
            viewmodel = new SnakeViewModel(model);

            timer = new DispatcherTimer();
            Int32 time = 400;
            timer.Interval = TimeSpan.FromMilliseconds(time);
            viewmodel.Interval = time;
            timer.Tick += View_Draw;

            viewmodel.GameOver += View_GameOver;
            viewmodel.ChangeTimer += View_ChangeTimer;
            viewmodel.OpenFile += View_OpenFile;
            viewmodel.SaveFile += View_SaveFile;
            viewmodel.Exit += View_Exit;

            viewmodel.GameStart(15);

            mainWindow.DataContext = viewmodel;
            mainWindow.Show();
        }

        private void View_Exit(object sender, EventArgs e)
        {
            Shutdown();
        }

        private async void View_SaveFile(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    await model.SaveAsync(saveFileDialog.FileName, model.Table);
                }
                catch (SnakeDataException)
                {
                    MessageBox.Show("Couldn't save the game!" + Environment.NewLine + "Couldn't access the directory.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void View_OpenFile(object sender, EventArgs e)
        {
 
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    await model.OpenAsync(openFileDialog.FileName);
                }
                catch (SnakeDataException)
                {
                    MessageBox.Show("Couldn't load game!" + Environment.NewLine + "Wrong path or file.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void View_GameOver(object sender, EventArgs e)
        {
            timer.Stop();
            MessageBox.Show($"The game ended! Time ellapsed: {(1.0 * model.Seconds / 1000).ToString("F2")} seconds. Snake length: {model.Snake.Count}.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void View_ChangeTimer(object sender, bool e)
        {
            timer.IsEnabled = e;
        }

        private void View_Draw(object sender, EventArgs e)
        {
            viewmodel.Model_Tick();
        }
    }
}
