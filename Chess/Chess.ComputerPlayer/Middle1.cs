using Chess.Entity;
using GraphAlgorithms.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ComputerPlayer
{
    public class Middle1 : AbstarctMiddle
    {
        public override string Name => "Middle 1";

        public record QueueRecord(Board board, long boardLevel = 0) { }

        private Queue<QueueRecord> boardsForCheck = new Queue<QueueRecord>();

        private long stepNumber = 0;

        public Middle1(Board board) : base(board) { }

        public override Step MakeStep(int deep)
        {
            stepNumber++;
            //TODO: Сделать давку офицерами и конями. В конце игра загон под мат/(шах и мат).

            // В случает отсутствия стратегии сделать простой шаг.
            return base.MakeStep(deep);
        }


        /// <summary>
        /// Добавляет доску в очередь.
        /// </summary>
        /// <param name="board">Доска</param>

        private void AddBoard(Board board, long boardLevel = 0)
        {
            boardsForCheck.Enqueue(new QueueRecord(board, boardLevel));
        }

        /// <summary>
        /// Очищает очередь.
        /// </summary>
        private void ClearQueue()
        {
            boardsForCheck.Clear();
        }
    }
}
