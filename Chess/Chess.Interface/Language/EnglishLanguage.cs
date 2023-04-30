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
                    ["Language"] = "Language",
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
            newGameSettings.ComputerName1.Items[0] = "5 Steps Strategy";
            newGameSettings.ComputerName2.Items[0] = "5 Steps Strategy";
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
                    ["BlackIsOnCheckmate"] = "Black Is On Checkmate",
                    ["WhiteIsOnCheckmate"] = "White Is On Checkmate",
                    ["BlackIsOnCheck"] = "Black Is On Check",
                    ["WhiteIsOnCheck"] = "White Is On Check",
                    ["BlackIsOnMate"] = "Black Is On Mate",
                    ["WhiteIsOnMate"] = "White Is On Mate",
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

                if (le.IsCheck) { logEntityTypeId = 1; }
                else if (le.IsMate) { logEntityTypeId = 2; }
                else if (le.IsCheckmate) { logEntityTypeId = 3; }

                string stepString = $"Step {step.Id}: {stepSide} from {Board.GetStringCellName((byte)step.Step.Start.X, (byte)step.Step.Start.Y)} in {Board.GetStringCellName((byte)step.Step.End.X, (byte)step.Step.End.Y)}";
                string leText = logEntityTypeId switch
                {
                    0 => stepString,
                    1 => $"{stepString}. {stepSide} is on Check!",
                    2 => $"{stepString}. {stepSide} is on Mate!",
                    3 => $"{stepString}. {stepSide} is on Checkmate!",
                    _ => "Log Error"
                };

                return leText;
            }

            return String.Empty;
        }

    }
}
