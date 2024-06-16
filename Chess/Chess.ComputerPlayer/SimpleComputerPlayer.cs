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
        static readonly Random random = new Random(234);

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

            // Первым делом съесть, что возможно
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

            // Код уклонения от удара. 
            // Ход не под другой удар. Если нет такого, то не важно.
            (byte lastPlayerX, byte lastPlayerY) = (newBoard.LastHumanStepPosition[0], newBoard.LastHumanStepPosition[1]); // Последний ход противоположной стороны
            var anotherPlayerLastStep = new CellPoint() { X = (sbyte)lastPlayerX, Y = (sbyte)lastPlayerY }; // Конвертируем в нужный тип данных
            var attackSteps = newBoard.GetAvailiableStepsWithoutCastlingForPre(anotherPlayerLastStep); // Получаем его ходы атаки
            Dictionary<CellPoint, List<CellPoint>> oppositeAvailableSteps = newBoard.GetAvailableSteps(Board.GetOppositeSide(newBoard.CurrentStepSide));
            foreach (var attacked in attackSteps)
            {
                if (newBoard.Positions[attacked.X, attacked.Y].Man != Figures.Empty && newBoard.Positions[attacked.X, attacked.Y].Side == newBoard.CurrentStepSide)
                {
                    var awaySteps = newBoard.GetAvailableSteps(newBoard.CurrentStepSide, new CellPoint() { X = (sbyte)attacked.X, Y = (sbyte)attacked.Y });

                    // Начальная фигура хода. Фигура в массиве одна.
                    CellPoint startFigure = awaySteps.Keys.ElementAt(0);

                    if (availableSteps.ContainsKey(startFigure))
                    {
                        // Концы хода
                        var steps = availableSteps[startFigure];
                        foreach (var availableStep in steps)
                        {
                            foreach (var figure in oppositeAvailableSteps)
                            {
                                foreach (var step in figure.Value)
                                {
                                    foreach (var reallyStep in steps)
                                    {
                                        if (availableStep.X == reallyStep.X && availableStep.Y == reallyStep.X)
                                        {
                                            continue; 
                                        }
                                        else
                                        {
                                            return new Step(startFigure, availableStep);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Если не съели и не уклонились, то ходим, но не под удар.
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

                    if (!IsItDangerous(stepCP)) {
                        return new Step(rootCP, stepCP);
                    }
                }
            }

            // Иначе, случайно ходим:
            CellPoint rootCPEnd = availableSteps.Keys.ElementAt(random.Next(availableSteps.Keys.Count - 1));
            CellPoint stepCPEnd = rootCPEnd;

            while (availableSteps[rootCPEnd].Count < 1)
            {
                rootCPEnd = availableSteps.Keys.ElementAt(random.Next(availableSteps.Keys.Count - 1));
            }

            for (int j = 0; j < availableSteps[rootCPEnd].Count; j++)
            {
                // Конец хода
                stepCPEnd = availableSteps[rootCPEnd]
                        .ToArray()[random.Next(availableSteps[rootCPEnd].Count - 1)];
                break;
            }

            return new Step(rootCPEnd, stepCPEnd);
        }


        // Возвращает true, если ход под удар.
        private bool IsItDangerous(CellPoint stepCP)
        {
            // Создаём пустой массив ходов (графов) с начальными позициями фигур
            var newBoard = new Board(board.ToByteArray());
            Dictionary<CellPoint, List<CellPoint>> availableSteps = newBoard.GetAvailableSteps(Board.GetOppositeSide(newBoard.CurrentStepSide));

            // Цикл съедания:
            for (int i = 0; i < availableSteps.Keys.Count; i++)
            {
                // Начальная фигура хода
                CellPoint rootCP = availableSteps.Keys.ElementAt(i);

                for (int j = 0; j < availableSteps[availableSteps.Keys.ElementAt(i)].Count; j++)
                {
                    // Конец хода
                    CellPoint stepCPEnd = availableSteps[rootCP]
                            .ToArray()[j];

                    if (stepCP.X == stepCPEnd.X && stepCP.Y == stepCPEnd.Y)
                    {
                        return true;
                    }
                }

            }

            return false;
        }
    }

}
