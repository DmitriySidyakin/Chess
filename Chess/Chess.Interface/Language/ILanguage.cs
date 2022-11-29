using Chess.Logging;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Chess.InterfaceTranslation
{
    public interface ILanguage
    {
        Dictionary<string, string> MainWindowStrings { get; }

        void MakeInterfaceTranslation(MainWindow mainWindow, NewGameSettings newGameSettings)
        {
            foreach (var el in MainWindowStrings)
            {
                // Перевод элементов главного окна
                var wndEl = mainWindow.FindName(el.Key);
                if (wndEl is UIElement)
                {
                    var wndElMenuItem = (wndEl as MenuItem);
                    if (wndElMenuItem is not null)
                        wndElMenuItem.Header = MainWindowStrings[wndElMenuItem.Name];
                }
            }
        }

        string MakeShortLogString(LogEntity le);
    }
}