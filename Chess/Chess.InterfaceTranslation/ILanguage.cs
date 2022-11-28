using System.Collections.Generic;

namespace Chess.InterfaceTranslation
{
    public interface ILanguage
    {
        Dictionary<string, string> Strings { get; }

        void MakeInterfaceTranslation(MainWindow mainWindow, NewGameSettings newGameSettings);
    }
}