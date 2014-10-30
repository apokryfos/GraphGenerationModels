using System; using GraphFramework.Interfaces; using GraphFramework; using GraphFramework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using DataStructures;

namespace GraphGenerationModelsSample.Interface {


	public abstract class RandomizedGraphGenerationBase<TVertex> : GraphGenerationBase<TVertex> {
		protected Random r = new Random();
		public RandomizedGraphGenerationBase(int totalNumberOfVertices, Func<int, TVertex> VertexFactory, params double[] parameters) :
		 base(totalNumberOfVertices, VertexFactory, parameters) {
			
		}

	}

	public enum GraphType { DIRECTED, BIDIRECTIONAL, UNDIRECTED }
	
	/// <summary>
	/// Base class for graph generation models.
	/// </summary>
	/// <typeparam name="TVertex">Vertex type</typeparam>
	public abstract class GraphGenerationBase<TVertex> : IChangesModel<TVertex> {

		public static GraphType DefaultGraphType = GraphType.UNDIRECTED;

		protected int totalNumberOfVertices;		
		protected double[] parameters;

		/// <summary>
		/// The default constructor
		/// </summary>
		/// <param name="totalNumberOfVertices">The total number of vertices to be generated</param>
		/// <param name="VertexFactory">A function which maps integers to TVertex types.</param>
		/// <param name="parameters">The model parameters</param>
		public GraphGenerationBase(int totalNumberOfVertices, Func<int, TVertex> VertexFactory, params double[] parameters) {
			this.totalNumberOfVertices = totalNumberOfVertices;
			this.VertexFactory = VertexFactory;
			this.parameters = parameters;
		}

		public double[] Parameters { get { return parameters; } }
		
		public	Func<int,TVertex> VertexFactory { get; set; }


		#region IChangesModel<TVertex>

		public abstract string ModelName {
			get;
		}

		public abstract ParameterDescriptor[] ParameterDescriptions {
			get;
		}

		private bool logStatus = false;
		private const string logFile = ".\\Generation.log";

		public bool LogStatusTexts {
			get { return logStatus; }
			set {
				if (logStatus == false && value == true) {
					File.Create(logFile);					
				}
					


				logStatus = value;

			}
		}

		protected void OnProgressTick(int v, int m) {
			if (ProgressTick != null)
				ProgressTick(this, new ChangesModelEventArgs(v, m));
		}

		public virtual event EventHandler<ChangesModelEventArgs> ProgressTick;
		
		
		
		public virtual void Initialize() {
			if (DefaultGraphType == GraphType.DIRECTED) {
				VisitedGraph = new AdjacencyGraph<TVertex>();
			} else if (DefaultGraphType == GraphType.BIDIRECTIONAL) {
				VisitedGraph = new BidirectionalGraph<TVertex>();
			} else {
				VisitedGraph = new UndirectedGraph<TVertex>(false);				
			}
		}
		

		public virtual void Compute() {
			Initialize();
			while (VisitedGraph.VertexCount < totalNumberOfVertices) {
				Step();
			}			
		}

		protected virtual void OnProgressTick() {
			OnProgressTick(VisitedGraph.VertexCount, VisitedGraph.EdgeCount);
		}

		#endregion

		public IAdjacencyGraph<TVertex> VisitedGraph { get; set; }
		public abstract void Step();

		protected virtual bool AddEdge(TVertex source, TVertex target) {
			return this.VisitedGraph.AddVerticesAndEdge(source, target);
		}
		protected virtual bool AddEdge(int source, int target) {
			return this.VisitedGraph.AddVerticesAndEdge(VertexFactory(source), VertexFactory(target));
		}

		protected virtual bool RemoveEdge(TVertex source, TVertex target) {
			return this.VisitedGraph.RemoveEdge(source, target);
		}
		protected virtual bool RemoveEdge(int source_index, int target_index) {
			return this.VisitedGraph.RemoveEdge(VertexFactory(source_index), VertexFactory(target_index));
		}


		public int VertexTarget {
			get { return totalNumberOfVertices; }
		}
	}
}
