using System.ComponentModel.DataAnnotations;

namespace GraphAlgorithms.Graph
{
    public class Edge<DataT>
    {
        public NodeWithEdges<DataT> To { get; init; }

        /// <summary>
        /// Данные грани
        /// </summary>
        public DataT? Data { get; set; }

        public Edge(NodeWithEdges<DataT> to)
        {
            To = to;
        }
    }
}
