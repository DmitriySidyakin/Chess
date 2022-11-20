﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Settings
{
    internal class GameSettings
    {
        public PlayerType Player1White { get; set; }

        public PlayerType Player2Black { get; set; }

        public string Player1WhiteName { get; set; } = string.Empty;

        public string Player2BlackName { get; set; } = string.Empty;

        public byte GameHardLevel { get; set; }
    }
}
