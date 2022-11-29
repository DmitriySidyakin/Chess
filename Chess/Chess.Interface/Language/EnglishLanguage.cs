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

        public Dictionary<string, string> NewGameWindowStrings => throw new NotImplementedException();

        public Dictionary<string, string> MessagesStrings => throw new NotImplementedException();

        public string MakeMousePositionMessage(CellPoint cellPoint)
        {
            return $"Mouse is in {Board.GetStringCellName((byte)cellPoint.X, (byte)cellPoint.Y)}{Environment.NewLine}";
        }

        public string MakeShortLogString(LogEntity le)
        {
            if(le is StepEntity step)
            {
                return $"Step {step.Id}: {Board.GetStringCellName((byte)step.Step.Start.X, (byte)step.Step.Start.Y)} in {Board.GetStringCellName((byte)step.Step.End.X, (byte)step.Step.End.Y)}";
            }
            return String.Empty;
        }
    }
}
