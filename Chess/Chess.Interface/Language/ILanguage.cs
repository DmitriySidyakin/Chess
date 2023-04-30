using Chess.Entity;
using Chess.Logging;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Chess.InterfaceTranslation
{
    public interface ILanguage
    {
        Dictionary<string, string> MainWindowStrings { get; }

        Dictionary<string, string> MessagesStrings { get; }

        void MakeInterfaceTranslation(MainWindow mainWindow, NewGameSettings newGameSettings)
        {
            TranslateMainWindowsMenu(mainWindow);
            NewGameWindowStrings(newGameSettings);
        }

        void TranslateMainWindowsMenu(MainWindow mainWindow)
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

        string MakeShortLogString(StepEntity le);
        string MakeMousePositionMessage(CellPoint cellPoint);

        string MakeStringMessage(string message)
        {
            return MessagesStrings[message];
        }

        void NewGameWindowStrings(NewGameSettings newGameSettings);
    }
}