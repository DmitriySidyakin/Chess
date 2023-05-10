using Chess.Entity;
using GraphAlgorithms.Graph;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;

namespace Chess.ComputerPlayer
{
    public class GraphStepPlayer : IComputerPlayer
    {
        public string Name => "GraphStepPlayer";

        Board board;
        Side currentStepSide;

        public GraphStepPlayer(Board board) { this.board = new Board(board.ToByteArray()); currentStepSide = board.CurrentStepSide; }

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


        // TODO: Fix a current step error
        public Step MakeStep(int deep)
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
                weightedGraphChessBoards[i] = new WeightedGraph<CellPoint>();
                var root = weightedGraphChessBoards[i].AddEmptyNode();
                root.Data = rootCP;

                for (int j = 0; j < availableSteps[availableSteps.Keys.ElementAt(i)].Count; j++)
                {
                    // Конец хода
                    CellPoint stepCP = availableSteps[rootCP]
                            .ToArray()[j];

                    /*if (IsAvailableStep(availableSteps, stepCP.X, stepCP.Y))
                    {*/
                    var step = weightedGraphChessBoards[i].AddEmptyNode();
                    step.Data = stepCP;
                    var edge = weightedGraphChessBoards[i].AddEdge(root, step, GetFigureWeight(stepCP));
                    MakeStep2(weightedGraphChessBoards[i], newBoard, rootCP, stepCP, deep);
                    /*}*/
                    GC.Collect();
                }

                (CellPoint? cpStart, var path, long w) = FindStep(weightedGraphChessBoards[i], newBoard);
                Step stepCurrent = new Step(cpStart, weightedGraphChessBoards[i].GetNode(1).Data);

                shortestPaths[i] = (stepCurrent, w);
            }

            int maxI = 0;
            if (shortestPaths.Count() > 0)
            {
                maxI = new Random().Next(shortestPaths.Count() - 1);
                long maxV = shortestPaths[maxI].Item2;
                for (int i = 0; i < shortestPaths.Length; i++)
                {
                    if (shortestPaths[i].Item2 > maxV)
                    {
                        maxI = i;
                        maxV = shortestPaths[i].Item2;
                    }
                }
            }
            else
                throw new GameEndedException();

            return shortestPaths[maxI].Item1;
        }

        /// <summary>
        /// Метод возвращает путь в графе с наименьшим весом.
        /// </summary>
        /// <param name="weightedGraph">Взвешенный граф ходов от начальной позации.</param>
        /// <returns>id в графе и его вес.</returns>
        private (CellPoint?, List<NodeWithWeightedEdges<CellPoint>>?, long) FindStep(WeightedGraph<CellPoint> weightedGraph, Board board)
        {
            // Которые являются to и не являются from
            List<int> finalStepsAndWeight = weightedGraph.GetLeafsAndRoot().Where(lr => lr.Id != 0).Select(lr => (int)lr.Id).ToList();;

            List<NodeWithWeightedEdges<CellPoint>>? path = null;
            long resultWeight = long.MaxValue;

            for (int i = 0; i < finalStepsAndWeight.Count; i++)
            {
                (var p, var w) = weightedGraph.FindAcyclicShortestPath(0, finalStepsAndWeight[i]);

                if (w != -1 && w < resultWeight)
                {
                    resultWeight = w;
                    path = p;
                }
            }

            return (weightedGraph.GetNode(0).Data, path/*GetStep(path)*/, resultWeight);

        }

        private long GetStep(List<NodeWithWeightedEdges<CellPoint>>? path)
        {
            if (path == null)
                throw new NotImplementedException();
            else
            {
                return path[1].Id;
            }
        }

        public void MakeStep2(WeightedGraph<CellPoint> weightedGraphChessBoard, Board board, CellPoint rootCP, CellPoint stepCP, int deep, int layer = 0)
        {
            if (layer == deep) return;

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
                    CellPoint stepCP2 = availableSteps[rootCP2]
                            .ToArray()[j];

                    /*if (IsAvailableStep(availableSteps, stepCP2.X, stepCP2.Y))
                    {*/
                    // Конец хода
                    var step = weightedGraphChessBoard.AddEmptyNode();
                    var edge = weightedGraphChessBoard.AddEdge(root, step, GetFigureWeight(stepCP2));
                    step.Data = stepCP2;
                    
                    layer++;
                    MakeStep2(weightedGraphChessBoard, board, rootCP2, stepCP2, layer);
                    /*}*/
                }
            }

        }

        /// <summary>
        /// Т.к. будем искать кратчайший путь, задаём вес съеденной фигуры с инверсией массы.
        /// </summary>
        /// <param name="cellPoint">Координаты съедаемой фигуры на доске.</param>
        /// <returns>Вес съеденной фигуры где король максимального веса съедания, с инверсией.</returns>
        /// <exception cref="NotImplementedException">Выдаётся, когда передана точка с неизвестным состоянием.</exception>
        private long GetFigureWeight(CellPoint cellPoint)
        {
            if (currentStepSide != board.Positions[cellPoint.X, cellPoint.Y].Side)
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