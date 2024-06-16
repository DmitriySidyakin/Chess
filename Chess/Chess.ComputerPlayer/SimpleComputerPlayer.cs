using Chess.Entity;
using GraphAlgorithms.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ComputerPlayer
{
    public class SimpleComputerPlayer : IComputerPlayer
    {
        Random random = new Random(234);

        public string Name => "SimpleComputerPlayer";

        Board board;
        Side currentStepSide;

        public SimpleComputerPlayer(Board board) { this.board = new Board(board.ToByteArray()); currentStepSide = board.CurrentStepSide; }

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

            // Цикл съедания:
            for (int i = 0; i < availableSteps.Keys.Count; i++)
            {
                // Начальная фигура хода
                CellPoint rootCP = availableSteps.Keys.ElementAt(i);

                for (int j = 0; j < availableSteps[availableSteps.Keys.ElementAt(i)].Count; j++)
                {
                    // Конец хода
                    CellPoint stepCP = availableSteps[rootCP]
                            .ToArray()[j];

                    if (board.Positions[stepCP.X, stepCP.Y].Side == Board.GetOppositeSide(newBoard.CurrentStepSide) && board.Positions[stepCP.X, stepCP.Y].Man != Figures.Empty)
                    {
                        return new Step(rootCP, stepCP);
                    }
                }

            }

            // Код уклонения от будующего удара. 
            // Отступаем в случайное место, возможно под другой удар.
            (byte lx, byte ly) = (board.LastHumanStepPosition[0], board.LastHumanStepPosition[1]); // Последний ход противоположной стороны
            var anotherPlayerLastStep = new CellPoint() { X = (sbyte)lx, Y = (sbyte)ly }; // Конвертируем в нужный тип данных
            var attackSteps = board.GetAvailiableStepsWithoutCastlingForPre(anotherPlayerLastStep); // Получаем его ходы атаки
            foreach (var attacked in attackSteps)
            {
                if (board.Positions[attacked.X, attacked.Y].Man != Figures.Empty && board.Positions[attacked.X, attacked.Y].Side == newBoard.CurrentStepSide) 
                {

                    var awaySteps = board.GetAvailableSteps(newBoard.CurrentStepSide, new CellPoint() { X = (sbyte)attacked.X, Y = (sbyte)attacked.Y });

                    // Начальная фигура хода
                    CellPoint rootCP = awaySteps.Keys.ElementAt(0);

                    
                    // Конец хода
                    CellPoint stepCP = availableSteps[rootCP].First();

                    if (board.Positions[stepCP.X, stepCP.Y].Man == Figures.Empty)
                    {
                        return new Step(rootCP, stepCP);
                    }
                }
            }

            // Если не съели и не уклонились, то случайно ходим, но не под удар.
            bool found = false;
            int maxIterations = 1000;
            int k = 0;
            while (!found && ++k < maxIterations)
            {


                // Начальная фигура хода
                CellPoint rootCP = availableSteps.Keys.ElementAt(random.Next(availableSteps.Keys.Count - 1));

                for (int j = 0; j < availableSteps[rootCP].Count; j++)
                {
                    // Конец хода
                    CellPoint stepCP = availableSteps[rootCP]
                            .ToArray()[j];

                    if (!IsItDangerous(stepCP)) { return new Step(rootCP, stepCP); }
                }
            }

            // Иначе, случайно ходим:
            CellPoint rootCPEnd = availableSteps.Keys.ElementAt(random.Next(availableSteps.Keys.Count - 1));
            CellPoint stepCPEnd = rootCPEnd;

            for (int j = 0; j < availableSteps[rootCPEnd].Count; j++)
            {
                // Конец хода
                stepCPEnd = availableSteps[rootCPEnd]
                        .ToArray()[j];
                break;
            }

            return new Step(rootCPEnd, stepCPEnd);
        }


        // Возвращает true, если ход под удар.
        private bool IsItDangerous(CellPoint stepCP)
        {
            (byte lx, byte ly) = (board.LastHumanStepPosition[0], board.LastHumanStepPosition[1]); // Последний ход противоположной стороны
            var anotherPlayerLastStep = new CellPoint() { X = (sbyte)lx, Y = (sbyte)ly }; // Конвертируем в нужный тип данных
            var attackedSteps = board.GetAvailiableStepsWithoutCastlingForPre(anotherPlayerLastStep); // Получаем его ходы атаки
            foreach (var attacked in attackedSteps)
            {
                if (stepCP.X == attacked.X && stepCP.Y == attacked.Y)
                {
                    return true;
                }
            }
            return false;
        }
    }

}
