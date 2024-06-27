using Chess.Interface;
using Chess.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chess
{
    /// <summary>
    /// Логика взаимодействия для NewGameSettings.xaml
    /// </summary>
    public partial class NewGameSettings : Window
    {
        public MainWindow mainWindow;

        public bool IsMainWindowClosing { get; set; } = false;

        public NewGameSettings(MainWindow mWindow)
        {
            mainWindow = mWindow;
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsMainWindowClosing)
            {
                // Cancel the closure
                e.Cancel = true;

                // Hide the window
                Hide();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized) return;
            ResetOppositePlayerIfIsItNeeded(e);
        }

        private void ResetOppositePlayerIfIsItNeeded(SelectionChangedEventArgs e)
        {
            if (e is not null)
            {
                if (e.AddedItems != null && e.AddedItems.Count == 1)
                {
                    string text = e?.AddedItems[0] is ComboBoxItem ? (e.AddedItems[0] as ComboBoxItem).Name : "";
                    PrepareForm(text);
                }
            }
        }

        private void PrepareForm(string text)
        {
            var selectedSideId = int.Parse(text.Last().ToString());

            if (!text.Contains("Player"))
            {
                if (selectedSideId > 1)
                {
                    ResetOppositeComboBoxIfIsItComputerPlayer(ComboBoxPlayer1, "1");

                    PlayerName2.Visibility = Visibility.Hidden;
                    ComputerName2.Visibility = Visibility.Visible;
                }
                else
                {
                    ResetOppositeComboBoxIfIsItComputerPlayer(ComboBoxPlayer2, "2");

                    PlayerName1.Visibility = Visibility.Hidden;
                    ComputerName1.Visibility = Visibility.Visible;
                }

                //SetHardLevelVisibility(Visibility.Visible);
            }
            else
            {
                if (selectedSideId > 1)
                {
                    PlayerName2.Visibility = Visibility.Visible;
                    ComputerName2.Visibility = Visibility.Hidden;
                }
                else
                {
                    PlayerName1.Visibility = Visibility.Visible;
                    ComputerName1.Visibility = Visibility.Hidden;
                }

                //SetHardLevelVisibility(Visibility.Hidden);
            }
        }

        private void SetHardLevelVisibility(Visibility visibility)
        {
            GameLevelLabel.Visibility = visibility;
            GameLevelSlider.Visibility = visibility;
            EasyLevelLabel.Visibility = visibility;
            HardLevelLabel.Visibility = visibility;
        }

        private void ResetOppositeComboBoxIfIsItComputerPlayer(ComboBox comboBox, string comboBoxId)
        {
            var comboBoxSelectedItem = comboBox.SelectedItem as ComboBoxItem;
            if (comboBoxSelectedItem is not null)
            {
                if (!comboBoxSelectedItem.Name.Contains("Player"))
                {
                    comboBox.SelectedItem = comboBox.Items.GetItemAt(0);
                }
            }
        }
        private ComputerType GetLevelOfComputerPlayer()
        {
            
            if (ComboBoxPlayer1.SelectedIndex == 1 /* Computer */)
            {
                switch (ComputerName1.SelectedIndex)
                {
                    case 0: return ComputerType.ForKids;
                    case 1: return ComputerType.SimpleComputerPlayer;
                    case 2: return ComputerType.Middle;
                    case 3: return ComputerType.MiddleStrategy1;
                }
            }
            else if (ComboBoxPlayer2.SelectedIndex == 1 /* Computer */) {
                switch (ComputerName2.SelectedIndex)
                {
                    case 0: return ComputerType.ForKids;
                    case 1: return ComputerType.SimpleComputerPlayer;
                    case 2: return ComputerType.Middle;
                    case 3: return ComputerType.MiddleStrategy1;
                }
            }

            // Не достижимо по логике
            return ComputerType.ForKids;
        }

        private void StartNewGameButton_Click(object sender, RoutedEventArgs e)
        {
            
            mainWindow.GameSettings.Player1White = PlayerName1.Visibility == Visibility.Visible ? Settings.PlayerType.Player : Settings.PlayerType.Computer;
            mainWindow.GameSettings.Player2Black = PlayerName2.Visibility == Visibility.Visible ? Settings.PlayerType.Player : Settings.PlayerType.Computer;

            mainWindow.GameSettings.Player1WhiteName = PlayerName1.Text;
            mainWindow.GameSettings.Player2BlackName = PlayerName2.Text;

            mainWindow.GameSettings.ComputerType = GetLevelOfComputerPlayer();

            this.Visibility = Visibility.Hidden;
            mainWindow.EndGame();
            mainWindow.ResetBoard();
            mainWindow.StartGame();
        }
    }
}
