using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;

namespace GraphGenerationModelsSample {
	public static class GraphMetrics {



		public static SortedDictionary<int, double> OutDegreeDistribution<TVertex>(this IAdjacencyGraph<TVertex> graph) {
			return DegreeDistribution(graph,(v => graph.OutDegree(v))) ;
		}
		public static SortedDictionary<int, double> InDegreeDistribution<TVertex>(this IBidirectionalGraph<TVertex> graph) {
			return DegreeDistribution(graph, (v => graph.InDegree(v)));
		}
		public static SortedDictionary<int, double> TotalDegreeDistribution<TVertex>(this IAdjacencyGraph<TVertex> graph) {
			return DegreeDistribution(graph, (v => graph.AdjacentDegree(v)));
		}


		private static SortedDictionary<int, double> DegreeDistribution<TVertex>(this IAdjacencyGraph<TVertex> graph, Func<TVertex, int> DegreeFunc) {
			SortedDictionary<int, double> DegreeDist = new SortedDictionary<int, double>();
			foreach (var v in graph.Vertices) {
				var degree = DegreeFunc(v);
				if (DegreeDist.ContainsKey(degree)) {
					DegreeDist[degree]++;
				} else {
					DegreeDist.Add(degree, 1);
				}
			}
			return DegreeDist;
		}

	}
}
