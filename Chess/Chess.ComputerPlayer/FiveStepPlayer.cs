using Chess.Entity;

namespace Chess.ComputerPlayer
{
    public class FiveStepPlayer : IComputerPlayer
    {
        public string Name => "FiveStepPlayer";

        public Board CurrentBoard { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Step MakeStep()
        {
            throw new NotImplementedException();
        }
    }
}