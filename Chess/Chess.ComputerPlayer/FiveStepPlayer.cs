using Chess.Entity;
using GraphAlgorithms.Graph;

namespace Chess.ComputerPlayer
{
    public class FiveStepPlayer : IComputerPlayer
    {
        public string Name => "FiveStepPlayer";

        Board board;
        Side currentStepSide;

        public FiveStepPlayer(Board board) { this.board = new Board(board.ToByteArray()); currentStepSide = board.CurrentStepSide; }

        public Board CurrentBoard { 
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

        public Step MakeStep()
        {
            // Создаём пустой массив ходов (графов) с начальными позициями фигур
            var newBoard = new Board(board.ToByteArray());
            Dictionary<CellPoint, List<CellPoint>> availableSteps = newBoard.GetAvailableSteps(currentStepSide);
            WeightedGraph[] weightedGraphChessBoards = new WeightedGraph[availableSteps.Keys.Count];

            (Step, long)[] shortestPaths = new (Step, long)[availableSteps.Keys.Count];

            for (int i = 0; i < availableSteps.Keys.Count; i++)
            {
                // Начальная фигура хода
                CellPoint rootCP = availableSteps.Keys.ElementAt(i);
                var root = weightedGraphChessBoards[i].AddEmptyNode();

                for (int j = 0; j < availableSteps[availableSteps.Keys.ElementAt(i)].Count; j++)
                {
                    // Конец хода
                    CellPoint stepCP = availableSteps[rootCP]
                            .ToArray()[j];

                    if (IsAvailableStep(availableSteps, stepCP.X, stepCP.Y))
                    {
                        var step = weightedGraphChessBoards[i].AddEmptyNode();
                        var edge = weightedGraphChessBoards[i].AddEdge(root, step, GetFigureWeight(stepCP));
                        MakeStep3(weightedGraphChessBoards[i], newBoard, rootCP, stepCP);
                    }
                    
                }

                shortestPaths[i] = FindStep(weightedGraphChessBoards[i]);
            }

            int maxI = 0;
            long maxV = shortestPaths[0].Item2;
            for(int i = 1; i < shortestPaths.Length; i++)
            {
                if (shortestPaths[i].Item2 > maxV)
                {
                    maxI = i;
                    maxV = shortestPaths[i].Item2;
                }
            }

            return shortestPaths[maxI].Item1;
        }

        /// <summary>
        /// Метод возвращает путь в графе с наименьшим весом.
        /// </summary>
        /// <param name="weightedGraph">Взвешенный граф ходов от начальной позации.</param>
        /// <returns>Шаг и его вес.</returns>
        private (Step, long) FindStep(WeightedGraph weightedGraph)
        {
            throw new NotImplementedException();
        }

        public void MakeStep3(WeightedGraph weightedGraphChessBoard, Board board, CellPoint rootCP, CellPoint stepCP, int layer = 0)
        {

            if (layer == 6) return;

            var newBoard = new Board(board.ToByteArray());
            newBoard.MakeStepWithoutChecking(rootCP, stepCP);

            Dictionary<CellPoint, List<CellPoint>> availableSteps = newBoard.GetAvailableSteps(currentStepSide);

            for (int i = 0; i < availableSteps.Keys.Count; i++)
            {
                // Начальная фигура хода
                CellPoint rootCP2 = availableSteps.Keys.ElementAt(i);
                var root = weightedGraphChessBoard.AddEmptyNode();

                for (int j = 0; j < availableSteps[availableSteps.Keys.ElementAt(i)].Count; j++)
                {
                    CellPoint stepCP2 = availableSteps[rootCP]
                            .ToArray()[j];

                    if (IsAvailableStep(availableSteps, stepCP2.X, stepCP2.Y))
                    {
                        // Конец хода
                        var step = weightedGraphChessBoard.AddEmptyNode();
                        var edge = weightedGraphChessBoard.AddEdge(root, step, GetFigureWeight(stepCP2));
                        MakeStep3(weightedGraphChessBoard, board, rootCP2, stepCP2);
                    }
                }
            }

            layer++;
        }

        /// <summary>
        /// Т.к. будем искать кратчайший путь, задаём вес съеденной фигуры с инверсией массы.
        /// </summary>
        /// <param name="cellPoint">Координаты съедаемой фигуры на доске.</param>
        /// <returns>Вес съеденной фигуры где король максимального веса съедания, с инверсией.</returns>
        /// <exception cref="NotImplementedException">Выдаётся, когда передана точка с неизвестным состоянием.</exception>
        private long GetFigureWeight(CellPoint cellPoint)
        {
            return board.Positions[cellPoint.X, cellPoint.Y].Man switch
            {
                Figures.Pawn => 50,
                Figures.Queen => 5,
                Figures.Knight => 25,
                Figures.Rook => 25,
                Figures.King => 0,
                Figures.Empty => 100,
                Figures.Bishop => 25,
                _ => throw new NotImplementedException(),
            };
        }
    }
}