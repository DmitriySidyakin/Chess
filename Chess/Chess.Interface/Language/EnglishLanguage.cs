using Chess.Entity;
using Chess.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;

namespace Chess.InterfaceTranslation
{
    public class EnglishLanguage : ILanguage
    {
        public Dictionary<string, string> MainWindowStrings
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    ["NewGame"] = "New Game",
                    ["MainWindowGameSettings"] = "Game Settings",
                    ["GameLog"] = "Game Log",
                    ["OpenGame"] = "Open Game",
                    ["SaveGame"] = "Save Game",
                    ["SelectLanguage"] = "Language",
                    /*
                    [""] = "",
                    [""] = "",
                    [""] = "",
                    [""] = "",
                    [""] = "",
                    [""] = "",
                    [""] = "",
                    [""] = "",
                    [""] = "",
                    [""] = "",
                    [""] = "",
                    [""] = "",
                    [""] = "",*/
                };
            }
        }

        public void NewGameWindowStrings(NewGameSettings newGameSettings)
        {
            newGameSettings.PlayerName1.Text = "White Player";
            newGameSettings.PlayerName2.Text = "Black Player";
            newGameSettings.Computer1.Content = "Computer Player";
            newGameSettings.Computer2.Content = "Computer Player";
            newGameSettings.Player1.Content = "Player";
            newGameSettings.Player2.Content = "Player";
            newGameSettings.ComputerName1.Items[0] = "For Kids";
            newGameSettings.ComputerName2.Items[0] = "For Kids";
            newGameSettings.ComputerName1.Items[1] = "Simple Computer Player";
            newGameSettings.ComputerName2.Items[1] = "Simple Computer Player";
            newGameSettings.ComputerName1.SelectedIndex = 0;
            newGameSettings.ComputerName2.SelectedIndex = 0;
            newGameSettings.StartNewGameButton.Content = "Start New Game";

            newGameSettings.EasyLevelLabel.Content = "Easy";
            newGameSettings.HardLevelLabel.Content = "Hard";
            newGameSettings.WhitePlayerLabel.Content = "White Player";
            newGameSettings.BlackPlayerLabel.Content = "Black Player";
            newGameSettings.WhitePlayerNameLabel.Content = "White Player Name";
            newGameSettings.BlackPlayerNameLabel.Content = "Black Player Name";
            newGameSettings.GameLevelLabel.Content = "Game Level";
        }

        public Dictionary<string, string> MessagesStrings
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    ["TheGameIsStarted"] = "The Game Is Started",
                    ["BlackIsOnCheckmate"] = "Black make Checkmate",
                    ["WhiteIsOnCheckmate"] = "White make Checkmate",
                    ["BlackIsOnCheck"] = "Black make Check",
                    ["WhiteIsOnCheck"] = "White make Check",
                    ["BlackIsOnMate"] = "Black make Mate",
                    ["WhiteIsOnMate"] = "White make Mate",
                    ["Draw"] = "Draw",
                    /*
                    [""] = "",
                    [""] = "",
                    [""] = "",
                    [""] = "",
                    [""] = "",
                    [""] = "",*/
                };
            }
        }

        public string MakeMousePositionMessage(CellPoint cellPoint)
        {
            return $"Mouse is in {Board.GetStringCellName((byte)cellPoint.X, (byte)cellPoint.Y)}{Environment.NewLine}";
        }

        public string MakeShortLogString(StepEntity le)
        {
            if (le is StepEntity step)
            {
                var stepSide = le.StartSide == Side.White ? "White" : "Black";

                int logEntityTypeId = 0;

                if (le.IsCheckmate) { logEntityTypeId = 1; }
                else if (le.IsMate) { logEntityTypeId = 2; }
                else if (le.IsCheck) { logEntityTypeId = 3; }

                string stepString = $"Step {step.Id}: {stepSide} is from {Board.GetStringCellName((byte)step.Step.Start.X, (byte)step.Step.Start.Y)} in {Board.GetStringCellName((byte)step.Step.End.X, (byte)step.Step.End.Y)}";
                string leText = logEntityTypeId switch
                {
                    0 => stepString,
                    3 => $"{stepString}. {stepSide} make Check!",
                    2 => $"{stepString}.\n{stepSide} make Mate!",
                    1 => $"{stepString}.\n{stepSide} make Checkmate!",
                    _ => "Log Error"
                };

                return leText;
            }

            return String.Empty;
        }

    }
}
