using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using GraphFramework.Serializers;
using GraphFramework.Algorithms;
using DataStructures;
using GraphFramework.Interfaces;
using GraphFramework.Algorithms.Search;


namespace GraphGenerationModelsSample {
	
	using Triangle = DataStructures.TriangleGeneric<int>;
	using GraphGenerationModelsSample.Models;
	using GraphGenerationModelsSample.Interface;	
	
	
	
	public class Program {		
	
		
		public static string graphName = "graph.csv";
		public static Random r = new Random();
		public static void Main(string[] args) {
			
            double[][] parameters = new double[][] {
				new double[] { 0, 3, 0, 3, 0, 0},		
            };

			PlotScriptGeneration.GenerateWithPlotScript(typeof(GeneralUndirectedPreferentialAttachment<int>), new int[] { 30000 }, parameters);
		}

		
		/// <summary>
		/// Tries to read an already generated graph or creates this graph if it does not exist.
		/// This does not write the graph to disk if it does not exist.
		/// </summary>
		/// <param name="pathbase">The path in which the graph resides</param>
		/// <param name="c">The generation model</param>
		/// <returns>The target filename of the graph</returns>
		public static string GetGraph(string pathbase, IChangesModel<int> c) {
			string qualifiedName = pathbase + Path.DirectorySeparatorChar + c.GetType().Name + "-" + c.VertexTarget.ToString();
			if (c.Parameters != null && c.Parameters.Length > 0) {
				for (int i = 0; i < c.Parameters.Length; i++) {
					qualifiedName += "-" + c.Parameters[i];
				}
			}
			string graphFilename = qualifiedName + Path.DirectorySeparatorChar + graphName;


			if (!Directory.Exists(pathbase)) {
				Directory.CreateDirectory(pathbase);
			}
			if (!File.Exists(graphFilename) && !File.Exists(qualifiedName + Path.DirectorySeparatorChar + "degreeDistribution.txt")) {
				c.Compute();
			} else if (File.Exists(graphFilename) && !File.Exists(qualifiedName + Path.DirectorySeparatorChar + "degreeDistribution.txt")) {
				c.VisitedGraph = GraphReader<int>.ReadGraphAsUndirected(graphFilename);				
			}
			return qualifiedName;
		}				

	}
}
