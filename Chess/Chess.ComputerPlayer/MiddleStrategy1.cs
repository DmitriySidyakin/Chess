using Chess.Entity;
using GraphAlgorithms.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ComputerPlayer
{
    public class MiddleStrategy1 : IComputerPlayer
    {
        static readonly Random random = new Random(234);

        public string Name => "Middle1";

        Board board;
        Side currentStepSide;

        Queue<Step>  stepsQueue = new Queue<Step>();

        public MiddleStrategy1(Board board) { 
            this.board = new Board(board.ToByteArray()); currentStepSide = board.CurrentStepSide;

        }

        public Board CurrentBoard
        {
            get
            {
                return board;
            }

            set
            {
                board = value;
            }
        }

        public Side CurrentStepSide
        {
            get
            {
                return currentStepSide;
            }

            set
            {
                currentStepSide = value;
            }
        }


        private bool IsAvailableStep(Dictionary<CellPoint, List<CellPoint>> availableSteps, sbyte x, sbyte y)
        {
            return availableSteps.Count(s => s.Key.X == x && s.Key.Y == y && s.Value.Count(v => v.X == x && v.Y == y) > 0) > 0;
        }

        public Step MakeStep(int deep)
        {
            // Создаём пустой массив ходов (графов) с начальными позициями фигур
            var newBoard = new Board(board.ToByteArray());
            Dictionary<CellPoint, List<CellPoint>> availableSteps = newBoard.GetAvailableSteps(newBoard.CurrentStepSide);

            if(stepsQueue.Count > 0)
            {
                Step step = stepsQueue.Dequeue();
                
                MakeStepsQueue();

                return step;
            }
            else
            {
                MakeStepsQueue();
                if (stepsQueue.Count > 0)
                {
                    Step step = stepsQueue.Dequeue();

                    MakeStepsQueue();

                    return step;
                }
            }

            throw new UnpossibleSituation();
        }

        private void MakeStepsQueue()
        {
            throw new NotImplementedException();
        }


        // Возвращает true, если ход под удар.
        private bool IsItDangerous(CellPoint stepCP)
        {
            // Создаём пустой массив ходов (графов) с начальными позициями фигур
            var newBoard = new Board(board.ToByteArray());
            Dictionary<CellPoint, List<CellPoint>> availableOppositeSteps = newBoard.GetAvailableSteps(Board.GetOppositeSide(newBoard.CurrentStepSide));

            // Цикл съедания:
            for (int i = 0; i < availableOppositeSteps.Keys.Count; i++)
            {
                // Начальная фигура хода
                CellPoint rootCP = availableOppositeSteps.Keys.ElementAt(i);

                for (int j = 0; j < availableOppositeSteps[availableOppositeSteps.Keys.ElementAt(i)].Count; j++)
                {
                    // Конец хода
                    CellPoint stepCPEnd = availableOppositeSteps[rootCP]
                            .ToArray()[j];

                    if (stepCP.X == stepCPEnd.X && stepCP.Y == stepCPEnd.Y)
                    {
                        return true;
                    }
                }

            }

            return false;
        }



        /// <summary>
        /// Возвращаем вес фигуры для съедания
        /// </summary>
        /// <param name="board">Состояние шахматной доски для которого определяется вес съедания фигуры по координатам.</param>
        /// <param name="cellPoint">Координаты съедаемой/исследуемой фигуры.</param>
        /// <returns>Вес удара фигы в клетке.</returns>
        /// <exception cref="NotImplementedException">Исключение выдаётся, если код фигы не существует. Исключение не возможно по правильному алгоритму, но сделано, чтобы все пути кода возвращали результат.</exception>
        private long GetFigureWeight(Board board, CellPoint cellPoint)
        {
            if (currentStepSide != board.Positions[cellPoint.X, cellPoint.Y].Side)
                return board.Positions[cellPoint.X, cellPoint.Y].Man switch
                {
                    Figures.Pawn => 50,
                    Figures.Queen => 1000,
                    Figures.Knight => 500,
                    Figures.Rook => 500,
                    Figures.King => 0,
                    Figures.Empty => 0,
                    Figures.Bishop => 500,
                    _ => throw new NotImplementedException(),
                };
            else
                return board.Positions[cellPoint.X, cellPoint.Y].Man switch
                {
                    Figures.Pawn => 0,
                    Figures.Queen => 0,
                    Figures.Knight => 0,
                    Figures.Rook => 0,
                    Figures.King => 0,
                    Figures.Empty => 0,
                    Figures.Bishop => 0,
                    _ => throw new NotImplementedException(),
                };
        }
    }
}
