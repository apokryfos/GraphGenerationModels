using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Interfaces;
using GraphFramework; using GraphFramework.Extensions;
using GraphGenerationModelsSample.Interface;

namespace GraphGenerationModelsSample.Models {
	public class GeneralUndirectedPreferentialAttachment<TVertex> : EdgeListUndirectedGraphGenerationBase<TVertex> {

		/// <summary>
		/// Generates a graph using the general undirected preferential attachment method also known as the 
		/// web-graph model (Cooper and Frieze "A general model of undirected web graphs" - 2001).
		/// </summary>
		/// <param name="totalNumberOfVertices">The total number of vertices that should be generated</param>
		/// <param name="parameters">
		/// Additional Parameters:
		/// All these functions will be invoked using the current vertex count a the int argument
		/// Expected Parameters:       
		/// Parameter 1: The probability of a new vertex
		/// Parameter 2: Procedure NEW: Number of edges to be added
		/// Parameter 3: Procedure NEW: Probability Edge targets are selected uniformly
		/// Parameter 4: Procedure OLD: Number of edges to be added
		/// Parameter 5: Procedure OLD: Probability old vertex will be selected uniformly
		/// Parameter 6: Procedure OLD: Probability Edge targets are selected uniformly		
		/// </param>
		public GeneralUndirectedPreferentialAttachment(int totalNumberOfVertices, Func<int, TVertex> VertexFactory, params double[] parameters) : base(totalNumberOfVertices, VertexFactory, parameters) {
		}

		
	
		protected int EdgesPerStep {
			get { return (int)parameters[1]; }
		}

		protected int EdgesPerStepOldVertex {
			get { return (int)parameters[3]; }
		}


		protected double NewVertexProbability {
			get { return 1.0-parameters[0]; }
		}

		protected TVertex SelectSourceVertex() {
			if (r.NextDouble() <= NewVertexProbability) {
				var nVertex = VertexFactory(VertexList.Count);
				return nVertex;
			} else {
				if (r.NextDouble() <= parameters[4]) {
					return UniformVertexSelection();					
				} else {
					return PreferentialVertexSelection();
				}
			}
		}

		

		protected TVertex SelectTargetVertex(double uniformProbability) {
			if (r.NextDouble() <= uniformProbability) {
				return UniformVertexSelection();
			} else {
				return PreferentialVertexSelection();
			}
		
		}

		protected TVertex SelectTargetVertex() {
			return SelectTargetVertex(parameters[2]);
		}

		protected TVertex SelectTargetVertexOldVertex() {
			return SelectTargetVertex(parameters[5]);
		}


		public override string ModelName {
			get { return "General Model Of The Web-Graph"; }
		}

		//Based on the paper of Cooper and Frieze "A general model of undirected web graphs" - 2001
		public override ParameterDescriptor[] ParameterDescriptions {
			get {
				return new ParameterDescriptor[] {
					
					ParameterDescriptor.CreateProbabilityParameterDescriptor("alpha=P(New)"),
					
					// New
					ParameterDescriptor.CreateParameterDescriptor("m",ParameterType.INT, 1,int.MaxValue),
					ParameterDescriptor.CreateProbabilityParameterDescriptor("beta"),

					//Old
					ParameterDescriptor.CreateParameterDescriptor("M",ParameterType.INT, 1,int.MaxValue),
					ParameterDescriptor.CreateProbabilityParameterDescriptor("gamma"),
					ParameterDescriptor.CreateProbabilityParameterDescriptor("delta")

				};
			}
		}


		public override void Initialize() {
			base.Initialize();
			AddEdge(VertexFactory(0), VertexFactory(0));

		}

		public override void Step() {
			TVertex source = SelectSourceVertex();

			if (!VisitedGraph.ContainsVertex(source)) {
				for (int i = 0; i < EdgesPerStep; i++)
					AddEdge(source, SelectTargetVertex());
			} else {
				for (int i = 0; i < EdgesPerStepOldVertex; i++)
					AddEdge(source, SelectTargetVertexOldVertex());
			}
			if (VisitedGraph.VertexCount % 1000 == 0)
				OnProgressTick();
		}


	}
}
