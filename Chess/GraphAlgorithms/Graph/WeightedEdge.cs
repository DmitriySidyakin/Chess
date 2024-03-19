using System;
using System.ComponentModel.DataAnnotations;

namespace GraphAlgorithms.Graph
{
    public class WeightedEdge<DataT>
    {
        public NodeWithWeightedEdges<DataT> To { get; init; }

        /// <summary>
        /// Данные грани
        /// </summary>
        public DataT? Data { get; set; }

        public long Weight { get; set; } = long.MaxValue;

        public WeightedEdge(NodeWithWeightedEdges<DataT> to) { To = to; }

        public WeightedEdge(NodeWithWeightedEdges<DataT> to, long weight) : this(to) { Weight = weight; }
    }
}
