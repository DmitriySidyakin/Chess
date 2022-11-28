using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.InterfaceTranslation
{
    public class EmptyLanguage : ILanguage
    {
        public Dictionary<string, string> Strings
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    ["NewGame"] = "",
                    ["GameLevel"] = "",
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
        };

        public void MakeInterfaceTranslation(MainWindow mainWindow, NewGameSettings newGameSettings)
        {

        }
    }
}
