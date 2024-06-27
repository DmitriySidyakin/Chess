using Chess.Entity;
using GraphAlgorithms.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ComputerPlayer
{
    public abstract class AbstarctMiddle
    {
        static readonly Random random = new Random(8234);

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
                long weightX = AbstarctMiddle.GetFigureWeight(board, x.End);
                long weightY = AbstarctMiddle.GetFigureWeight(board, y.End);
                return weightY.CompareTo(weightX);
            }
        }

        public virtual string Name => "AbstarctMiddle";

        internal Board board;
        internal Side currentStepSide;

        public AbstarctMiddle(Board board) { this.board = new Board(board); currentStepSide = board.CurrentStepSide;  }

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
       public virtual Step MakeStep(int deep)
        {
            //TODO: Реализовать давку офицерами и конями с прогнозом на 3 шага в перёд.

            // В случает отсутствия стратегии сделать простой шаг.
            return MakeSimpleStep();
        }

        private Step MakeSimpleStep()
        {
            // Создаём пустой массив ходов (графов) с начальными позициями фигур
            var newBoard = new Board(board);
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
            eatSteps = eatSteps.Where(s => !IsItDangerous(s)).ToList();

            // Сортируем по важности съеденной фигуры, первая самая важная для съедания.
            eatSteps.Sort(new StepComparer(newBoard, currentStepSide));

            if (eatSteps.Count > 0)
                return eatSteps.First();

            // Уклоняемся.
            var lastOppositeStep = new CellPoint() { X = (sbyte)newBoard.LastHumanStepPosition[2], Y = (sbyte)newBoard.LastHumanStepPosition[3] }; // Последний ход противоположной стороны
            Dictionary<CellPoint, List<CellPoint>> oppositeAvailableSteps = newBoard.GetAvailableSteps(Board.GetOppositeSide(newBoard.CurrentStepSide));
            var lastPlayerStep = oppositeAvailableSteps.Where((i) => i.Key == lastOppositeStep);
            List<Step> resultAwaySteps = new();
            if (lastPlayerStep.Count() > 0)
            {
                var attackSteps = newBoard.GetAvailiableStepsWithoutCastlingForPre(lastOppositeStep); // Получаем его ходы атаки

                foreach (var attacked in attackSteps)
                {
                    if (newBoard.Positions[attacked.X, attacked.Y].Man != Figures.Empty && newBoard.Positions[attacked.X, attacked.Y].Side == newBoard.CurrentStepSide)
                    {
                        var awaySteps = newBoard.GetAvailableSteps(newBoard.CurrentStepSide, attacked);

                        // Начальная фигура хода. Фигура в массиве ключей одна.
                        CellPoint startFigure = awaySteps.Keys.ElementAt(0);

                        if (availableSteps.ContainsKey(startFigure))
                        {
                            // Концы хода
                            var steps = availableSteps[startFigure];
                            foreach (var aStep in steps)
                            {
                                if (!IsItDangerous(new Step(startFigure, aStep)))
                                    resultAwaySteps.Add(new Step(startFigure, aStep));
                            }
                        }
                    }
                }
            }
            if (resultAwaySteps.Count > 0)
                return resultAwaySteps[random.Next(resultAwaySteps.Count - 1)];

            // Если не съели и не уклонились, то ходим, но не под удар.
            List<Step> resultsRandomSteps = new();
            foreach (CellPoint rootCP in availableSteps.Keys)
            {
                for (int j = 0; j < availableSteps[rootCP].Count; j++)
                {
                    // Конец хода
                    CellPoint stepCP = availableSteps[rootCP]
                            .ToArray()[j];

                    if (!IsItDangerous(new Step(rootCP, stepCP)))
                    {
                        resultsRandomSteps.Add(new Step(rootCP, stepCP));
                    }
                }
            }
            if (resultsRandomSteps.Count > 0)
                return resultsRandomSteps[random.Next(resultsRandomSteps.Count - 1)];

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
        private bool IsItDangerous(Step step)
        {
            // Создаём пустой массив ходов (графов) с начальными позициями фигур
            var newBoard = new Board(board);

            newBoard.MakeStepWithoutChecking(new CellPoint() { X = step.Start.X, Y = step.Start.Y }, new CellPoint() { X = step.End.X, Y = step.End.Y });

            Dictionary<CellPoint, List<CellPoint>> availableOppositeSteps = newBoard.GetAvailableSteps(Board.GetOppositeSide(newBoard.CurrentStepSide));

            // Цикл съедания:
            for (int i = 0; i < availableOppositeSteps.Keys.Count; i++)
            {
                // Начальная фигура хода
                CellPoint rootCP = availableOppositeSteps.Keys.ElementAt(i);

                for (int j = 0; j < availableOppositeSteps[rootCP].Count; j++)
                {
                    // Конец хода
                    CellPoint stepCPEnd = availableOppositeSteps[rootCP]
                            .ToArray()[j];

                    if(step is not null)
                    if (step.Equals(stepCPEnd))
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
