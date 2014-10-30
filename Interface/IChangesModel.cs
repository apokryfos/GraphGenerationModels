using System; using GraphFramework.Interfaces; using GraphFramework; using GraphFramework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphFramework.Algorithms.Framework;




namespace GraphGenerationModelsSample.Interface {

	//The graph generation model state changed event arguments 
	public class ChangesModelEventArgs : EventArgs {
		private int _currentVertices;
		private int _currentEdges;

		public int CurrentVertices {
			get { return _currentVertices; }
		}
		public int CurrentEdges {
			get { return _currentEdges; }
		}

		private ChangesModelEventArgs() { }
		public ChangesModelEventArgs(int currentVertices, int currentEdges) {
			_currentEdges = currentEdges;
			_currentVertices = currentVertices;

		}
	}

	
	public interface IChangesModel<TVertex> : IAlgorithm<TVertex>, IIncrementalAlgorithm<TVertex> {
		string ModelName { get; }
		double[] Parameters { get; }
		int VertexTarget { get; }
		ParameterDescriptor[] ParameterDescriptions { get; }
		Func<int, TVertex> VertexFactory { get; set; }
		event EventHandler<ChangesModelEventArgs> ProgressTick;		
	}

	public interface IWeightedChangesModel<TVertex, TWeight> : IChangesModel<TVertex> {
		IWeighted<TVertex, TWeight> WeightsMap { get; }
	}
}
