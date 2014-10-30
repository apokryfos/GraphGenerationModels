using System; using GraphFramework.Interfaces; using GraphFramework; using GraphFramework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace GraphGenerationModelsSample.Models {
	
	public class PreferentialAttachementSimple<TVertex> : GeneralUndirectedPreferentialAttachment<TVertex> {
		
		/// <summary>
		/// Generates a graph using the undirected simple preferential attachment method 		
		/// I.E. The general undirected preferential attachment with fixed number of new edges per step and always using the NEW procedure
		/// Equivalent to the BA Graph generation model. 
		/// </summary>
		/// <param name="totalNumberOfVertices">The total number of vertices that should be generated</param>
		/// <param name="VertexFactory">A method which converts a vertex index to a vertex</param>
		/// <param name="parameters">		
		/// Expected Parameters:       
		/// Parameter 1: Number of edges to be added per step
		/// </param>
		public PreferentialAttachementSimple(int totalNumberOfVertices, Func<int, TVertex> VertexFactory, params double[] parameters)
			: base(totalNumberOfVertices, VertexFactory, 0.0, parameters[0], 0.0,0.0,0.0, 0.0) {			
		}
	}
}
