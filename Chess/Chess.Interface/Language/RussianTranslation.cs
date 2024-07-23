using Chess.Entity;
using Chess.Logging;
using System;
using System.Collections.Generic;

namespace Chess.InterfaceTranslation
{
    public class RussianTranslation : ILanguage
    {
        public Dictionary<string, string> MainWindowStrings
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    ["NewGame"] = "Новая игра",
                    ["MainWindowGameSettings"] = "Настройки игры",
                    ["GameLog"] = "Лог игры",
                    ["OpenGame"] = "Открыть игру",
                    ["SaveGame"] = "Сохранить игру",
                    ["SelectLanguage"] = "Язык",
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
                    [""] = "",
                };
            }
        }

        public void NewGameWindowStrings(NewGameSettings newGameSettings)
        {
            newGameSettings.PlayerName1.Text = "Белый Игрок";
            newGameSettings.PlayerName2.Text = "Чёрный игрок";
            newGameSettings.Computer1.Content = "Компьютерный игрок";
            newGameSettings.Computer2.Content = "Компьютерный игрок";
            newGameSettings.Player1.Content = "Игрок";
            newGameSettings.Player2.Content = "Игрок";
            newGameSettings.ComputerName1.Items[0] = "Для детей";
            newGameSettings.ComputerName2.Items[0] = "Для детей";
            newGameSettings.ComputerName1.Items[1] = "Простой компьютерный игрок";
            newGameSettings.ComputerName2.Items[1] = "Простой компьютерный игрок";
            newGameSettings.ComputerName1.Items[2] = "Средний (не реализовано)";
            newGameSettings.ComputerName2.Items[2] = "Средний (не реализовано)";
            newGameSettings.ComputerName1.Items[3] = "Средний 2 (не реализовано)";
            newGameSettings.ComputerName2.Items[3] = "Средний 2 (не реализовано)";

            newGameSettings.ComputerName1.SelectedIndex = 0;
            newGameSettings.ComputerName2.SelectedIndex = 0;
            newGameSettings.StartNewGameButton.Content = "Начать новую игру";

            newGameSettings.EasyLevelLabel.Content = "Легко";
            newGameSettings.HardLevelLabel.Content = "Сложно";
            //newGameSettings.EasyLevelLabel.ActualWidth = ;
            newGameSettings.HardLevelLabel.Margin = new(newGameSettings.HardLevelLabel.Margin.Left - 20, newGameSettings.HardLevelLabel.Margin.Top, newGameSettings.HardLevelLabel.Margin.Right, newGameSettings.HardLevelLabel.Margin.Bottom);
            newGameSettings.WhitePlayerLabel.Content = "Белый игрок";
            newGameSettings.BlackPlayerLabel.Content = "Чёрный игрок";
            newGameSettings.WhitePlayerNameLabel.Content = "Имя игрока за белый";
            newGameSettings.BlackPlayerNameLabel.Content = "Имя игрока за чёрных";
            newGameSettings.GameLevelLabel.Content = "Уровень сложности";
        }

        public Dictionary<string, string> MessagesStrings
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    ["TheGameIsStarted"] = "Игра начата",
                    ["BlackIsOnCheckmate"] = "Чёрные поставили шах и мат",
                    ["WhiteIsOnCheckmate"] = "Белые поставили шах и мат",
                    ["BlackIsOnCheck"] = "Чёрные поставили шах",
                    ["WhiteIsOnCheck"] = "Белые поставили шах",
                    ["BlackIsOnMate"] = "Чёрные поставили мат",
                    ["WhiteIsOnMate"] = "Белые поставили мат",
                    ["Draw"] = "Ничья",
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
            return $"Мышка на {Board.GetStringCellName((byte)cellPoint.X, (byte)cellPoint.Y)}{Environment.NewLine}";
        }

        public string MakeShortLogString(StepEntity le)
        {
            if (le is StepEntity step)
            {
                var stepSide = le.StartSide == Side.White ? "Белые" : "Чёрные";

                int logEntityTypeId = 0;

                if(le.IsCheckmate) { logEntityTypeId = 1; }
                else if(le.IsMate) { logEntityTypeId = 2; }
                else if(le.IsCheck) { logEntityTypeId = 3; }

                string stepString = $"Шаг {step.Id}: {stepSide} c {Board.GetStringCellName((byte)step.Step.Start.X, (byte)step.Step.Start.Y)} на {Board.GetStringCellName((byte)step.Step.End.X, (byte)step.Step.End.Y)}";
                string leText = logEntityTypeId switch
                {
                    0 => stepString,
                    3 => $"{stepString}. {stepSide} поставили Шах!",
                    2 => $"{stepString}.\n{stepSide} поставили Мат!",
                    1 => $"{stepString}.\n{stepSide} поставили Шах и Мат!",
                    _ => "Log Error"
                };

                return leText;
            }

            return String.Empty;
        }
    }
}