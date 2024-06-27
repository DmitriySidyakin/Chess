using Chess.Entity;
using GraphAlgorithms.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ComputerPlayer
{
    public class Middle : IComputerPlayer
    {
        static readonly Random random = new Random(234);

        public class StepComparer : IComparer<Step>
        {
            Board board;
            Side currentStepSide;

            public StepComparer(Board board, Side currentStepSide)
            {
                this.board = board;
                this.currentStepSide = currentStepSide;
            }

            int IComparer<Step>.Compare(Step? x, Step? y)
            {
                long weightX = Middle.GetFigureWeight(board, x.End);
                long weightY = Middle.GetFigureWeight(board, y.End);
                return weightY.CompareTo(weightX);
            }
        }

        public string Name => "Middle";

        Board board;
        Side currentStepSide;

        public Middle(Board board) { this.board = new Board(board.ToByteArray()); currentStepSide = board.CurrentStepSide; }

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
            List<Step> eatSteps = new();
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
                        eatSteps.Add(new Step(rootCP, stepCP));
                    }
                }
            }

            // Удаляем съедание под удар:
            eatSteps = eatSteps.Where(s => s.End != null ? IsItDangerous(s.End) : false).ToList();

            // Сортируем по важности съеденной фигуры, первая самая важная для съедания.
            eatSteps.Sort(new StepComparer(newBoard, currentStepSide));

            //
            if(eatSteps.Count > 0 )
                return eatSteps.First();

            // Уклоняемся от удара последнего хода. Если нет такого, то не важно.
            (byte lastPlayerX, byte lastPlayerY) = (newBoard.LastHumanStepPosition[0], newBoard.LastHumanStepPosition[1]); // Последний ход противоположной стороны
            Dictionary<CellPoint, List<CellPoint>> oppositeAvailableSteps = newBoard.GetAvailableSteps(Board.GetOppositeSide(newBoard.CurrentStepSide));
            var lastPlayerStep = oppositeAvailableSteps.Where((i) => i.Key.X == (sbyte)lastPlayerX && i.Key.Y == (sbyte)lastPlayerY);
            List<Step> resultAwaySteps = new();
            if (lastPlayerStep.Count() > 0)
            {
                var anotherPlayerLastStep = oppositeAvailableSteps.Where((i) => i.Key.X == (sbyte)lastPlayerX && i.Key.Y == (sbyte)lastPlayerY).First(); // Конвертируем в нужный тип данных
                var attackSteps = newBoard.GetAvailiableStepsWithoutCastlingForPre(anotherPlayerLastStep.Key); // Получаем его ходы атаки

                foreach (var attacked in attackSteps)
                {
                    if (newBoard.Positions[attacked.X, attacked.Y].Man != Figures.Empty && newBoard.Positions[attacked.X, attacked.Y].Side == newBoard.CurrentStepSide)
                    {
                        var awaySteps = newBoard.GetAvailableSteps(newBoard.CurrentStepSide, attacked);

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
                                            {
                                                resultAwaySteps.Add(new Step(startFigure, availableStep));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (resultAwaySteps.Count > 0)
                return resultAwaySteps[random.Next(resultAwaySteps.Count - 1)];

            // Если не съели и не уклонились, то ходим, но не под удар.
            bool found = false;
            int maxIterations = 10000;
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

                    if (!IsItDangerous(stepCP))
                    {
                        found = true;
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
        private static long GetFigureWeight(Board board, CellPoint cellPoint)
        {
            if (board.CurrentStepSide != board.Positions[cellPoint.X, cellPoint.Y].Side)
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
