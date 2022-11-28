using System.Collections.Generic;

namespace Chess.InterfaceTranslation
{
    public class RussianTranlsation : ILanguage
    {
        public Dictionary<string, string> Strings
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    ["NewGame"] = "Новая игра",
                    ["GameLevel"] = "Сложность игры",
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

        public void MakeInterfaceTranslation(MainWindow mainWindow, NewGameSettings newGameSettings)
        {

        }
    }
}