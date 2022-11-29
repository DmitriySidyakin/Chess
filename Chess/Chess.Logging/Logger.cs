using Chess.Settings;

namespace Chess.Logging
{
    public class Logger
    {
        public List<LogEntity> log = new List<LogEntity>();

        public GameSettings gameSettings;

        public Logger(GameSettings gameSettings)
        {
            this.gameSettings = gameSettings;
        }

        public Logger Add(LogEntity logEntity)
        {
            log.Add(logEntity);
            return this;
        }


    }
}
