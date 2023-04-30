using Chess.Settings;

namespace Chess.Logging
{
    public class Logger
    {
        public List<StepEntity> log = new List<StepEntity>();

        public GameSettings gameSettings;

        public Logger(GameSettings gameSettings)
        {
            this.gameSettings = gameSettings;
        }

        public Logger Add(StepEntity logEntity)
        {
            log.Add(logEntity);
            return this;
        }


    }
}
