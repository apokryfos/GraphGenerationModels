using System; using GraphFramework.Interfaces; using GraphFramework; using GraphFramework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using DataStructures;

namespace GraphGenerationModelsSample.Interface {

	public abstract class EdgeListDirectedGraphGenerationBase<TVertex> : EdgeListUndirectedGraphGenerationBase<TVertex> {

		public EdgeListDirectedGraphGenerationBase(int totalNumberOfVertices, Func<int, TVertex> VertexFactory, params double[] parameters)
			: base(totalNumberOfVertices, VertexFactory, parameters) {
		
		}

		protected override void SetGraphType() {
			DefaultGraphType = GraphType.BIDIRECTIONAL;
		}

		protected override TVertex PreferentialVertexSelection() {
			return PreferentialVertexSelection(true);
		}

		protected TVertex PreferentialVertexSelection(bool outDirection) {
			int index = r.Next(EdgeList.Count);
			if (outDirection) { return EdgeList[index].Source; } else { return EdgeList[index].Target; }
		}

	
	}

	public abstract class EdgeListUndirectedGraphGenerationBase<TVertex> : GraphGenerationBase<TVertex> {

		IList<Edge<TVertex>> edgeList;

		public EdgeListUndirectedGraphGenerationBase(int totalNumberOfVertices, Func<int, TVertex> VertexFactory, params double[] parameters) : base(totalNumberOfVertices, VertexFactory, parameters) {
		
		}

		protected virtual void SetGraphType() {
			DefaultGraphType = GraphType.UNDIRECTED;
		}

		public override void Initialize() {
			SetGraphType();
			base.Initialize();			
			edgeList = new VeryLargeList<Edge<TVertex>>();
			VisitedGraph.VertexAdded += new VertexAction<TVertex>(VisitedGraph_VertexAdded);
			VisitedGraph.EdgeAdded += new EdgeAction<TVertex>(VisitedGraph_EdgeAdded);

		}

		void VisitedGraph_EdgeAdded(IGraph<TVertex> graph, TVertex source, TVertex target) {
			edgeList.Add(new Edge<TVertex>(source, target));
		}

		void VisitedGraph_VertexAdded(IGraph<TVertex> graph, TVertex vertex) {
			VertexList.Add(vertex);
		}

		protected Random r = new Random();

		public IList<TVertex> VertexList = new List<TVertex>();
		public IList<Edge<TVertex>> EdgeList {
			get { return edgeList; }
		}

		protected virtual TVertex UniformVertexSelection() {
			return VertexList[r.Next(VertexList.Count)];
		}

		protected virtual TVertex PreferentialVertexSelection() {
			int index = r.Next(EdgeList.Count);
			if (r.NextDouble() <= 0.5) { return EdgeList[index].Source; } else { return EdgeList[index].Target; }
		}

		

		protected override bool AddEdge(TVertex source, TVertex target) {
			if (VisitedGraph.AddVertex(source)) { VertexList.Add(source); }
			if (VisitedGraph.AddVertex(target)) { VertexList.Add(target); }
			return VisitedGraph.AddEdge(source, target);
		}
		protected override bool RemoveEdge(TVertex source, TVertex target) {
			return VisitedGraph.RemoveEdge(source, target);			
		}
		
	}
}
