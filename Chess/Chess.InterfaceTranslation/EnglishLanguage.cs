using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Ribbon;

namespace Chess.InterfaceTranslation
{
    public class EnglishLanguage : ILanguage
    {
        public Dictionary<string, string> Strings
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    ["NewGame"] = "New Game",
                    ["GameLevel"] = "Game Level",
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

        public void MakeInterfaceTranslation(MainWindow mainWindow, NewGameSettings newGameSettings)
        {
            foreach(var el in Strings)
            {
                var wndEl = mainWindow.FindName(el.Key);
                if(wndEl is UIElement)
                {
                    var wndElRibbonButton = (wndEl as RibbonButton);
                    if(wndElRibbonButton is not null)
                        wndElRibbonButton.Label = el.Value;

                    var wndElMenuItem = (wndEl as MenuIte);
                }
            }
        }
    }
}
