using Chess.Entity;
using Chess.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.InterfaceTranslation
{
    public class EmptyLanguage : ILanguage
    {
        public Dictionary<string, string> MainWindowStrings
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    ["NewGame"] = "",
                    ["MainWindowGameSettings"] = "",
                    ["GameLog"] = "",
                    ["OpenGame"] = "",
                    ["SaveGame"] = "",
                    ["Language"] = "",
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
            //TODO : Add code in future

        }
        public Dictionary<string, string> MessagesStrings => throw new NotImplementedException();

        public string MakeMousePositionMessage(CellPoint cellPoint)
        {
            return $"Mouse is in {Board.GetStringCellName((byte)cellPoint.X, (byte)cellPoint.Y)}{Environment.NewLine}";
        }

        public string MakeShortLogString(LogEntity le)
        {
            throw new NotImplementedException();
        }
    }
}
