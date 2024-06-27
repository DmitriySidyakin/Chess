using System.Runtime.Serialization;

namespace Chess.ComputerPlayer
{
    [Serializable]
    public class UnpossibleSituation : Exception
    {
        public UnpossibleSituation()
        {
        }

        public UnpossibleSituation(string? message) : base(message)
        {
        }

        public UnpossibleSituation(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnpossibleSituation(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}