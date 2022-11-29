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
                    ["GameSettings"] = "",
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

        public string MakeShortLogString(LogEntity le)
        {
            throw new NotImplementedException();
        }
    }
}
