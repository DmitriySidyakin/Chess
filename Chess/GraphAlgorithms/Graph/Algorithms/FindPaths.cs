using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphAlgorithms.Graph
{
    public partial class Graph<DataT>
    {
        public List<(List<NodeWithEdges<DataT>>, long)> FindAcyclicPaths(long nodeIdStart, long nodeIdEnd)
        {
            NodeWithEdges<DataT> startNode = GetNode(nodeIdStart);
            NodeWithEdges<DataT> endNode = GetNode(nodeIdEnd);

            List<(List<NodeWithEdges<DataT>>, long)> paths = FindAcyclicPathsRecursively(0, new List<NodeWithEdges<DataT>>(), startNode, endNode);

            return paths;
        }

        public (List<NodeWithEdges<DataT>>, long) FindAcyclicShortestPath(long nodeIdStart, long nodeIdEnd)
        {
            List<(List<NodeWithEdges<DataT>>, long)> paths = FindAcyclicPaths(nodeIdStart, nodeIdEnd);
            //PrintPaths(paths);
            (List<NodeWithEdges<DataT>>, long) shortestAcyclicPath;
            try
            {
                shortestAcyclicPath = paths.MinBy((i) => i.Item2);
            }
            catch (Exception)
            {
                return (new(), -1);
            }

            return shortestAcyclicPath;
        }

        #region Debug methods

        protected void PrintPaths(List<(List<NodeWithEdges<DataT>>, long)> paths)
        {
            long i = 0;
            foreach (var (list, weight) in paths)
            {
                Console.Write($"Path {i}, W = {weight}: ");

                foreach (var node in list)
                {
                    Console.Write($"{node.Id}, ");
                }
                Console.WriteLine();
                i++;
            }
        }

        #endregion

        private List<(List<NodeWithEdges<DataT>>, long)> FindAcyclicPathsRecursively(long startPathWeight, List<NodeWithEdges<DataT>> currentPath, NodeWithEdges<DataT> startNode, NodeWithEdges<DataT> endNode)
        {
            currentPath.Add(startNode);

            List<(List<NodeWithEdges<DataT>>, long)> acyclicPaths = new List<(List<NodeWithEdges<DataT>>, long)>();

            long resultWeight = startPathWeight;

            if (startNode.Id != endNode.Id && startNode.EdgeCount > 0)
            {
                for (var i = 0; i < startNode.EdgeCount; i++)
                {
                    var edge = startNode.Edges[i];

                    // Избежание циклических связей
                    if (currentPath.Contains(edge.To))
                    {
                        continue;
                    }

                    acyclicPaths.AddRange(FindAcyclicPathsRecursively(resultWeight + 1 /* или вместо 1 вес ребра */, new List<NodeWithEdges<DataT>>(currentPath), edge.To, endNode));
                }

                return acyclicPaths;
            }
            else
            {

                if (endNode.Id != startNode.Id)   
                    return acyclicPaths;

                acyclicPaths.Add((currentPath, resultWeight));
                return acyclicPaths;
            }
        }
    }

    public partial class WeightedGraph<DataT>
    {
        public List<(List<NodeWithWeightedEdges<DataT>>, long)> FindAcyclicPaths(long nodeIdStart, long nodeIdEnd)
        {
            NodeWithWeightedEdges<DataT> startNode = GetNode(nodeIdStart);
            NodeWithWeightedEdges<DataT> endNode = GetNode(nodeIdEnd);
            
            List<(List<NodeWithWeightedEdges<DataT>>, long)> paths = FindAcyclicPathsRecursively(0, new List<NodeWithWeightedEdges<DataT>>(), startNode, endNode);

            return paths;
        }

        public (List<NodeWithWeightedEdges<DataT>>, long) FindAcyclicShortestPath(long nodeIdStart, long nodeIdEnd)
        {
            List<(List<NodeWithWeightedEdges<DataT>>, long)> paths = FindAcyclicPaths(nodeIdStart, nodeIdEnd);
            //PrintPaths(paths);
            (List<NodeWithWeightedEdges<DataT>>, long) shortestAcyclicPath;
            try
            {
                shortestAcyclicPath = paths.MinBy((i) => i.Item2);
            }
            catch (Exception)
            {
                return (new(), -1);
            }

            return shortestAcyclicPath;
        }

        #region Debug methods

        protected void PrintPaths(List<(List<NodeWithWeightedEdges<DataT>>, long)> paths)
        {
            long i = 0;
            foreach(var (list, weight) in paths)
            {
                Console.Write($"Path {i}, W = {weight}: ");
                    
                foreach(var node in list)
                {
                    Console.Write($"{node.Id}, ");
                }
                Console.WriteLine();
                i++;
            }
        }

        #endregion

        private List<(List<NodeWithWeightedEdges<DataT>>, long)> FindAcyclicPathsRecursively(
            long startPathWeight,
            List<NodeWithWeightedEdges<DataT>> currentPath,
            NodeWithWeightedEdges<DataT> startNode,
            NodeWithWeightedEdges<DataT> endNode)
        {
            currentPath.Add(startNode);

            List<(List<NodeWithWeightedEdges<DataT>>, long)> acyclicPaths = new List<(List<NodeWithWeightedEdges<DataT>>, long)>();

            long resultWeight = startPathWeight;

            if (startNode.Id != endNode.Id && startNode.EdgeCount > 0)
            {
                for (var i = 0; i < startNode.EdgeCount; i++)
                {
                    var edge = startNode.WeightedEdges[i];

                    // Избежание циклических связей
                    if (currentPath.Contains(edge.To))
                    {
                        continue;
                    }
                        
                    acyclicPaths.AddRange(
                        FindAcyclicPathsRecursively(resultWeight + edge.Weight /* или 1 в невзвешенном графе */
                        , new List<NodeWithWeightedEdges<DataT>>(currentPath), edge.To, endNode)
                        ); 
                }

                return acyclicPaths;
            }
            else
            {
                if (endNode.Id != startNode.Id)
                    return acyclicPaths;

                acyclicPaths.Add((currentPath, resultWeight));
                return acyclicPaths;
            }
        }

    }

    public partial class MultiDirectedGraph<DataT>
    {
        public new List<(List<NodeWithEdges<DataT>>, long)> FindAcyclicPaths(long nodeIdStart, long nodeIdEnd)
        {
            NodeWithEdges<DataT> startNode = GetNode(nodeIdStart);
            NodeWithEdges<DataT> endNode = GetNode(nodeIdEnd);

            List<(List<NodeWithEdges<DataT>>, long)> paths = FindAcyclicPathsRecursively(0, new List<NodeWithEdges<DataT>>(), startNode, endNode);
            //PrintPaths(paths);
            return paths;
        }

        public new (List<NodeWithEdges<DataT>>, long) FindAcyclicShortestPath(long nodeIdStart, long nodeIdEnd)
        {
            List<(List<NodeWithEdges<DataT>>, long)> paths = FindAcyclicPaths(nodeIdStart, nodeIdEnd);

            (List<NodeWithEdges<DataT>>, long) shortestAcyclicPath;
            try
            {
                shortestAcyclicPath = paths.MinBy((i) => i.Item2);
            }
            catch (Exception)
            {
                return (new(), -1);
            }

            return shortestAcyclicPath;
        }

        #region Debug methods

        protected new void PrintPaths(List<(List<NodeWithEdges<DataT>>, long)> paths)
        {
            long i = 0;
            foreach (var (list, weight) in paths)
            {
                Console.Write($"Path {i}, W = {weight}: ");

                foreach (var node in list)
                {
                    Console.Write($"{node.Id}, ");
                }
                Console.WriteLine();
                i++;
            }
        }

        #endregion

        private List<(List<NodeWithEdges<DataT>>, long)> FindAcyclicPathsRecursively(long startPathWeight, List<NodeWithEdges<DataT>> currentPath, NodeWithEdges<DataT> startNode, NodeWithEdges<DataT> endNode)
        {
            currentPath.Add(startNode);

            List<(List<NodeWithEdges<DataT>>, long)> acyclicPaths = new List<(List<NodeWithEdges<DataT>>, long)>();

            long resultWeight = startPathWeight;

            if (startNode.Id != endNode.Id && startNode.EdgeCount > 0)
            {
                for (var i = 0; i < startNode.EdgeCount; i++)
                {
                    var edge = startNode.Edges[i];

                    // Избежание циклических связей
                    if (currentPath.Contains(edge.To))
                    {
                        continue;
                    }
                    acyclicPaths.AddRange(FindAcyclicPathsRecursively(resultWeight + 1, new List<NodeWithEdges<DataT>>(currentPath), edge.To, endNode));
                }

                return acyclicPaths;
            }
            else
            {
                if (endNode.Id != startNode.Id)
                    return acyclicPaths;

                acyclicPaths.Add((currentPath, resultWeight));
                return acyclicPaths;
            }
        }
    }

    public partial class MultiDirectedWeightedGraph<DataT>
    {
        public new List<(List<NodeWithWeightedEdges<DataT>>, long)> FindAcyclicPaths(long nodeIdStart, long nodeIdEnd)
        {
            NodeWithWeightedEdges<DataT> startNode = GetNode(nodeIdStart);
            NodeWithWeightedEdges<DataT> endNode = GetNode(nodeIdEnd);

            List<(List<NodeWithWeightedEdges<DataT>>, long)> paths = FindAcyclicPathsRecursively(0, new List<NodeWithWeightedEdges<DataT>>(), startNode, endNode);
            //PrintPaths(paths);
            return paths;
        }

        public new (List<NodeWithWeightedEdges<DataT>>, long) FindAcyclicShortestPath(long nodeIdStart, long nodeIdEnd)
        {
            List<(List<NodeWithWeightedEdges<DataT>>, long)> paths = FindAcyclicPaths(nodeIdStart, nodeIdEnd);

            (List<NodeWithWeightedEdges<DataT>>, long) shortestAcyclicPath;
            try
            {
                shortestAcyclicPath = paths.MinBy((i) => i.Item2);
            }
            catch (Exception)
            {
                return (new(), -1);
            }

            return shortestAcyclicPath;
        }

        #region Debug methods

        protected new void PrintPaths(List<(List<NodeWithWeightedEdges<DataT>>, long)> paths)
        {
            long i = 0;
            foreach (var (list, weight) in paths)
            {
                Console.Write($"Path {i}, W = {weight}: ");

                foreach (var node in list)
                {
                    Console.Write($"{node.Id}, ");
                }
                Console.WriteLine();
                i++;
            }
        }

        #endregion

        private List<(List<NodeWithWeightedEdges<DataT>>, long)> FindAcyclicPathsRecursively(long startPathWeight, List<NodeWithWeightedEdges<DataT>> currentPath, NodeWithWeightedEdges<DataT> startNode, NodeWithWeightedEdges<DataT> endNode)
        {
            currentPath.Add(startNode);

            List<(List<NodeWithWeightedEdges<DataT>>, long)> acyclicPaths = new List<(List<NodeWithWeightedEdges<DataT>>, long)>();

            long resultWeight = startPathWeight;

            if (startNode.Id != endNode.Id && startNode.EdgeCount > 0)
            {
                for (var i = 0; i < startNode.EdgeCount; i++)
                {
                    var edge = startNode.WeightedEdges[i];

                    // Избежание циклических связей
                    if (currentPath.Contains(edge.To))
                    {
                        continue;
                    }

                    acyclicPaths.AddRange(FindAcyclicPathsRecursively(resultWeight + edge.Weight, new List<NodeWithWeightedEdges<DataT>>(currentPath), edge.To, endNode));
                }

                return acyclicPaths;
            }
            else
            {
                if (endNode.Id != startNode.Id)
                    return acyclicPaths;

                acyclicPaths.Add((currentPath, resultWeight));
                return acyclicPaths;
            }
        }
    }
}
