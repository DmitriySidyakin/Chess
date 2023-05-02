using Chess.Entity;
using GraphAlgorithms.Graph;
using System.Globalization;

namespace Chess.ComputerPlayer
{
    public class FiveStepPlayer : IComputerPlayer
    {
        public string Name => "FiveStepPlayer";

        Board board;
        Side currentStepSide;

        public FiveStepPlayer(Board board) { this.board = new Board(board.ToByteArray()); currentStepSide = board.CurrentStepSide; }

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

        public Step MakeStep()
        {
            // Создаём пустой массив ходов (графов) с начальными позициями фигур
            var newBoard = new Board(board.ToByteArray());
            Dictionary<CellPoint, List<CellPoint>> availableSteps = newBoard.GetAvailableSteps(newBoard.CurrentStepSide);
            WeightedGraph<CellPoint>[] weightedGraphChessBoards = new WeightedGraph<CellPoint>[availableSteps.Keys.Count];

            (Step, long)[] shortestPaths = new (Step, long)[availableSteps.Keys.Count];

            for (int i = 0; i < availableSteps.Keys.Count; i++)
            {
                // Начальная фигура хода
                CellPoint rootCP = availableSteps.Keys.ElementAt(i);
                var root = weightedGraphChessBoards[i].AddEmptyNode();
                root.Data = rootCP;

                for (int j = 0; j < availableSteps[availableSteps.Keys.ElementAt(i)].Count; j++)
                {
                    // Конец хода
                    CellPoint stepCP = availableSteps[rootCP]
                            .ToArray()[j];

                    if (IsAvailableStep(availableSteps, stepCP.X, stepCP.Y))
                    {
                        var step = weightedGraphChessBoards[i].AddEmptyNode();
                        step.Data = stepCP;
                        var edge = weightedGraphChessBoards[i].AddEdge(root, step, GetFigureWeight(stepCP));
                        MakeStep2(weightedGraphChessBoards[i], newBoard, rootCP, stepCP);
                    }

                }

                (var path, long w) = FindStep(weightedGraphChessBoards[i]);
                Step stepCurrent = new Step(weightedGraphChessBoards[i].GetNode(path?[0].Id ?? 0).Data, weightedGraphChessBoards[i].GetNode(path?[1].Id ?? 1).Data);

                shortestPaths[i] = (stepCurrent, w);
            }

            int maxI = 0;
            long maxV = shortestPaths[0].Item2;
            for (int i = 1; i < shortestPaths.Length; i++)
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
        /// <returns>id в графе и его вес.</returns>
        private (List<NodeWithWeightedEdges<CellPoint>>?, long) FindStep(WeightedGraph<CellPoint> weightedGraph)
        {
            // Которые являются to и не являются from
            List<int> finalStepsAndWeight = new();
            for (int i = 0; i < weightedGraph.NodeCount; i++)
            {
                try
                {
                    weightedGraph.GetEdge(i, 0);
                }
                catch
                {
                    finalStepsAndWeight.Add(i);
                }
            }

            List<NodeWithWeightedEdges<CellPoint>>? path = null;
            long resultWeight = long.MaxValue;

            for(int i = 0; i < finalStepsAndWeight.Count; i++)
            {
                (var p, var w) = weightedGraph.FindAcyclicShortestPath(0, finalStepsAndWeight[i]);
                
                if(w < resultWeight)
                { 
                    resultWeight = w;
                    path = p;
                }
            }

            return (path/*GetStep(path)*/, resultWeight);

        }

        private long GetStep(List<NodeWithWeightedEdges<CellPoint>>? path)
        {
            if(path == null)
                throw new NotImplementedException();
            else
            {
                return path[1].Id;
            }
        }

        public void MakeStep2(WeightedGraph<CellPoint> weightedGraphChessBoard, Board board, CellPoint rootCP, CellPoint stepCP, int layer = 0)
        {

            if (layer == 6) return;

            var newBoard = new Board(board.ToByteArray());
            newBoard.MakeStepWithoutChecking(rootCP, stepCP);

            Dictionary<CellPoint, List<CellPoint>> availableSteps = newBoard.GetAvailableSteps(newBoard.CurrentStepSide);

            for (int i = 0; i < availableSteps.Keys.Count; i++)
            {
                // Начальная фигура хода
                CellPoint rootCP2 = availableSteps.Keys.ElementAt(i);
                var root = weightedGraphChessBoard.AddEmptyNode();
                root.Data = rootCP2;

                for (int j = 0; j < availableSteps[availableSteps.Keys.ElementAt(i)].Count; j++)
                {
                    CellPoint stepCP2 = availableSteps[rootCP]
                            .ToArray()[j];

                    if (IsAvailableStep(availableSteps, stepCP2.X, stepCP2.Y))
                    {
                        // Конец хода
                        var step = weightedGraphChessBoard.AddEmptyNode();
                        step.Data = stepCP2;
                        var edge = weightedGraphChessBoard.AddEdge(root, step, GetFigureWeight(stepCP2));
                        MakeStep2(weightedGraphChessBoard, board, rootCP2, stepCP2);
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
            if(currentStepSide != board.Positions[cellPoint.X, cellPoint.Y].Side)
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
            else
                return board.Positions[cellPoint.X, cellPoint.Y].Man switch
                {
                    Figures.Pawn => 200,
                    Figures.Queen => 2000,
                    Figures.Knight => 1000,
                    Figures.Rook => 1000,
                    Figures.King => 5000,
                    Figures.Empty => 0,
                    Figures.Bishop => 1000,
                    _ => throw new NotImplementedException(),
                };
        }
    }
}