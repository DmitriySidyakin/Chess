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
                    ["Language"] = "Язык",
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

        public Dictionary<string, string> NewGameWindowStrings => throw new NotImplementedException();

        public Dictionary<string, string> MessagesStrings => throw new NotImplementedException();

        public string MakeMousePositionMessage(CellPoint cellPoint)
        {
            return $"Мышка на {Board.GetStringCellName((byte)cellPoint.X, (byte)cellPoint.Y)}{Environment.NewLine}";
        }

        public string MakeShortLogString(LogEntity le)
        {
            if (le is StepEntity step)
            {
                return $"Шаг {step.Id}: {Board.GetStringCellName((byte)step.Step.Start.X, (byte)step.Step.Start.Y)} на {Board.GetStringCellName((byte)step.Step.End.X, (byte)step.Step.End.Y)}";
            }
            return String.Empty;
        }
    }
}