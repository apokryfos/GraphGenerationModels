using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GraphFramework.Serializers;
using GraphFramework.Interfaces;
using GraphFramework;
using GraphGenerationModelsSample.Interface;

namespace GraphGenerationModelsSample {

	public struct GenerationParameters {
		public Type GenerationType { get; set; }
		public int Size { get; set; }
		public IEnumerable<double[]> ParameterSets { get; set; } 
	}



	public static class PlotScriptGeneration {

		private static void SaveGraph(string qualifiedName, IChangesModel<int> c) {
			SaveGraph(qualifiedName, c, false);
		}

		/// <summary>
		/// Saves a graph and it's degree distributions. If the graph is directed then the in,out and total degree distributions are saved,
		/// otherwise only the (total) degree distribution.
		/// </summary>
		/// <param name="qualifiedName">The target filename</param>
		/// <param name="c">The graph model</param>		
		/// <param name="refreshDegrees">Whether the degree distribution files should be rewritten even if they already exist</param>
		private static void SaveGraph(string qualifiedName, IChangesModel<int> c, bool refreshDegrees) {
			string graphFilename = qualifiedName + Path.DirectorySeparatorChar + Program.graphName;

			if (!Directory.Exists(qualifiedName)) {
				Directory.CreateDirectory(qualifiedName);
			}
			if (!File.Exists(graphFilename)) {
				GraphWriter<int>.WriteGraph(c.VisitedGraph, graphFilename);
			}



			if ((c.VisitedGraph == null && refreshDegrees)) {
				c.VisitedGraph = GraphReader<int>.ReadGraphAsBidirectional(graphFilename);
			}
			
			if (c.VisitedGraph != null && c.VisitedGraph.VertexCount > 0) {			
				if (!File.Exists(qualifiedName + Path.DirectorySeparatorChar + "degreeDistribution.txt") || refreshDegrees) {
					SortedDictionary<int, double> DegreeDistributions = c.VisitedGraph.TotalDegreeDistribution();
					SortedDictionary<int, int> Degrees = new SortedDictionary<int, int>();
					using (StreamWriter sw = new StreamWriter(qualifiedName + Path.DirectorySeparatorChar + "degreeDistribution.txt")) {
						foreach (var kv in DegreeDistributions) {
							sw.WriteLine(Math.Log(kv.Key, c.VisitedGraph.VertexCount) + "\t" + kv.Value / c.VisitedGraph.VertexCount);
						}
					}
				}
				var bgraph = c.VisitedGraph as IBidirectionalGraph<int>;
				if (bgraph != null) {
					if (!File.Exists(qualifiedName + Path.DirectorySeparatorChar + "Din.txt") || refreshDegrees) {
						SortedDictionary<int, double> DegreeDistributions = bgraph.InDegreeDistribution();
						SortedDictionary<int, int> Degrees = new SortedDictionary<int, int>();
						using (StreamWriter sw = new StreamWriter(qualifiedName + Path.DirectorySeparatorChar + "Din.txt")) {
							foreach (var kv in DegreeDistributions) {
								sw.WriteLine(Math.Log(kv.Key, c.VisitedGraph.VertexCount) + "\t" + kv.Value / c.VisitedGraph.VertexCount);
							}
						}
					}
				}
				var agraph = c.VisitedGraph as IUndirectedGraph<int>;
				if (agraph == null) {
					if (!File.Exists(qualifiedName + Path.DirectorySeparatorChar + "Dout.txt") || refreshDegrees) {
						SortedDictionary<int, double> DegreeDistributions = c.VisitedGraph.OutDegreeDistribution();
						if (DegreeDistributions.Count > 3) {
							using (StreamWriter sw = new StreamWriter(qualifiedName + Path.DirectorySeparatorChar + "Dout.txt")) {
								foreach (var kv in DegreeDistributions) {
									sw.WriteLine(Math.Log(kv.Key, c.VisitedGraph.VertexCount) + "\t" + kv.Value / c.VisitedGraph.VertexCount);
								}
							}
						}
					}
				}
			}

		}

		/// <summary>
		/// Generates graphs with the given parameter sets and writes a GnuPlot script which, when ran, will plot the degree distribution of all generated graphs
		/// </summary>
		/// <param name="graphGenerationTypes">The graph generation parameters (model, size etc.) for all graphs to be generated</param>
		public static void GenerateWithPlotScript(IEnumerable<GenerationParameters> graphGenerationTypes) {
			using (StreamWriter sw = new StreamWriter(GetBasePath() + Path.DirectorySeparatorChar + "plotAll.plt")) {
				sw.WriteLine("set term pngcairo dashed color enhanced size 2600,1230 font ',26'");
				sw.WriteLine("set border 15 lw 5");
				sw.WriteLine("set xlabel '{/Symbol e}=\\log_n d(u)'");
				sw.WriteLine("set ylabel 'Frequency'");
				sw.WriteLine("f(A,c,n,x) = A/((n**x)**c)");
				sw.WriteLine("set logscale y");
				sw.WriteLine("set yrange [1.0/(" + ((graphGenerationTypes.Max(gt => gt.Size)) + 100) + ") to 1.0]");
				sw.WriteLine("FIT_LIMIT = 1e-9");
				sw.WriteLine("set output 'plotall.png'");
				foreach (var gt in graphGenerationTypes) {
					sw.WriteLine(GenerateWithPlotScript(gt));
				}
			}
		}


		

		/// <summary>
		/// Generates a graph with the given parameters and writes a GnuPlot script which, when ran, will plot the degree distribution of this graph
		/// </summary>
		/// <param name="graphGenerationType">The graph generation parameters (model, size etc.)</param>
		/// <returns>A string to the generated graph filename</returns>
		public static string GenerateWithPlotScript(GenerationParameters graphGenerationType) {

			int i = graphGenerationType.Size;
			int j = 0;
			StringBuilder sb = new StringBuilder();

			int paramIndex = -1;
			string[] outputdirs = new string[graphGenerationType.ParameterSets.Count()];
			string[] titles = new string[graphGenerationType.ParameterSets.Count()];
			string cparamName = "c" + i + paramIndex;
			string AparamName = "A" + i + paramIndex;

			foreach (var parameters in graphGenerationType.ParameterSets) {
				paramIndex++;
				
				IChangesModel<int> c = (IChangesModel<int>)Activator.CreateInstance(graphGenerationType.GenerationType, i, new Func<int, int>(v => v), parameters);
				var outputdir = Program.GetGraph(GetBasePath(), c);
				SaveGraph(outputdir, c,true);
				cparamName = "c" + i + paramIndex;
				AparamName = "A" + i + paramIndex;


				using (StreamWriter sw = new StreamWriter(outputdir + Path.DirectorySeparatorChar + "plot.plt")) {
					sw.WriteLine("set term pngcairo dashed color enhanced size 2600,1230 font ',26'");
					sw.WriteLine("f(A,c,n,x) = A/((n**x)**c)");
					sw.WriteLine("set border 15 lw 5");
					sw.WriteLine("set xlabel '{/Symbol e}=\\log_n d(u)'");
					sw.WriteLine("set ylabel 'Frequency'");
					string title = c.ModelName + " \\n n=" + i;
					sw.WriteLine("set title \"Degree Distributions for " + title + "\"");
					if (paramIndex == 0) {
						sb.AppendLine("set title \"Degree Distributions for " + title + "\"");
					}
					sw.WriteLine("set logscale y");
					
					sw.WriteLine("set yrange [1.0/(" + (i + 100) + ") to 1.0]");


					sw.WriteLine("set output 'g-" + i + "-" + j + ".png'");
					string[] paramnames = new string[parameters.Length];
					string title2 = "";
					for (int p = 0; p < parameters.Length; p++) {
						if (c.ParameterDescriptions[p].Type == ParameterType.DOUBLE || c.ParameterDescriptions[p].Type == ParameterType.PROBABILITY) {
							paramnames[p] = c.ParameterDescriptions[p].Name + " = " + string.Format("{0:00.00}", parameters[p]);
						} else if (c.ParameterDescriptions[p].Type == ParameterType.INT) {
							paramnames[p] = c.ParameterDescriptions[p].Name + " = " + (int)parameters[p];
						} else {
							paramnames[p] = (parameters[p]==0?"Not ":" ") + c.ParameterDescriptions[p].Name;
						}
						title2 += (p > 0 ? ", " : "") + paramnames[p];
					}




					sw.WriteLine("c=3");
					sw.WriteLine("A = 100");
					sw.WriteLine("FIT_LIMIT = 1e-9");
					sw.WriteLine("fit [x=0.2 to 0.3] f(A,c,"+i+",x) '" + outputdir + "\\degreeDistribution.txt' using 1:2 via A,c");

					

					sb.AppendLine(cparamName + "=3");
					sb.AppendLine(AparamName + " = 100");
					sb.AppendLine("fit [x=0.2 to 0.3] f("+AparamName + ","+cparamName + ","+i+",x) '" + outputdir + "\\degreeDistribution.txt' using 1:2 via " + AparamName + ","+ cparamName);

					if (File.Exists(outputdir + Path.DirectorySeparatorChar + "Din.txt")) {
						sw.WriteLine("cin=3");
						sw.WriteLine("Ain = 100");
						sw.WriteLine("fit [x=0.2 to 0.3] f(Ain,cin,"+i+",x) '" + outputdir + "\\Din.txt' using 1:2 via Ain,cin");
					}
					if (File.Exists(outputdir + Path.DirectorySeparatorChar + "Dout.txt")) {
						sw.WriteLine("cout=3");
						sw.WriteLine("Aout = 100");
						sw.WriteLine("fit [x=0.2 to 0.3] f(Aout,cout," + i + ",x) '" + outputdir + "\\Dout.txt' using 1:2 via Aout,cout");
					}


					sw.WriteLine("plot '" + outputdir + "\\degreeDistribution.txt' using 1:2 with linespoints lw 5 ps 2 title '" + title2 + ", Total Degree',\\");
					sw.Write("f(A,c,"+i+",x) lw 5 title sprintf(\"f(x) = A/x^{%2.2f})\",c)");


					if (File.Exists(outputdir + Path.DirectorySeparatorChar + "Din.txt")) {
						sw.WriteLine(",\\" + Environment.NewLine + "'" + outputdir + "\\Din.txt' using 1:2 with linespoints lw 5 ps 2 title 'In Degree',\\");
						sw.Write("f(Ain,cin," + i + ",x) lw 5 title sprintf(\"f(x) = A/x^{%2.2f})\",cin)");
					}
					if (File.Exists(outputdir + Path.DirectorySeparatorChar + "Dout.txt")) {
						sw.WriteLine(",\\" + Environment.NewLine + "'" + outputdir + "\\Dout.txt' using 1:2 with linespoints lw 5 ps 2 title 'Out Degree',\\");
						sw.Write("f(Aout,cout," + i + ",x) lw 5 title sprintf(\"f(x) = A/x^{%2.2f})\",cout)");
					}
					sw.WriteLine(";");

					sw.WriteLine("set output");
					outputdirs[paramIndex] = outputdir;
					titles[paramIndex] = title2;
				}
			}
			
			paramIndex = 0;
			sb.Append("plot ");
			foreach (var p in graphGenerationType.ParameterSets) {
				cparamName = "c" + i + paramIndex;
				AparamName = "A" + i + paramIndex;
				sb.AppendLine("'" + outputdirs[paramIndex] + "\\degreeDistribution.txt' using 1:2 with linespoints lw 5 ps 2 title '" + titles[paramIndex] + ", Total Degree',\\");
				sb.Append("f(" + AparamName + "," + cparamName + ","+i+",x) lw 5 title sprintf(\"f(x) = A/x^{%2.2f})\"," + cparamName + ")");
				paramIndex++;
				sb.AppendLine((paramIndex == graphGenerationType.ParameterSets.Count() ? ";" : ",\\"));
			}
			sb.AppendLine("set output");

			j++;
			GC.Collect();
			
			return sb.ToString();
		}


		/// <summary>
		/// Generates graphs using a given generation model.
		/// It will generate a graph for each combination of sizes and parameters given for a total of Ns.Count*parameterSets.Count() graphs.
		/// </summary>
		/// <param name="graphGenerationType">The generation model to use</param>
		/// <param name="Ns">The sizes of the graphs to be generated</param>
		/// <param name="parameterSets">The parameters used to generate these graphs</param>
		public static void GenerateWithPlotScript(Type graphGenerationType, IEnumerable<int> Ns, IEnumerable<double[]> parameterSets) {

			List<GenerationParameters> gp = new List<GenerationParameters>();
			foreach (var i in Ns) {
				gp.Add(new GenerationParameters { GenerationType = graphGenerationType, Size = i, ParameterSets = parameterSets });
			}
			GenerateWithPlotScript(gp);
			Console.WriteLine("Press any key to continue... Press any other key to quit");
			Console.ReadLine();		
		}

		//Gets a default path for the graphs to be stored in
		public static string GetBasePath() {
			var di = DriveInfo.GetDrives();
			//-- Uncomment this to always use the first physical drive which is not the root drive. May not work in Mono.NET
			//var targetDrive = di.FirstOrDefault(d => !d.Name.Contains("C:") && d.DriveType == DriveType.Fixed); 
			var targetDrive = di.FirstOrDefault();
			if (targetDrive == default(DriveInfo)) {
				targetDrive = di.First();
			}
			var pathbase = targetDrive.RootDirectory.FullName + Path.DirectorySeparatorChar + @"Data\Generation\";

			if (!Directory.Exists(pathbase)) {
				Directory.CreateDirectory(pathbase);
			}
			return pathbase;

		}
	}
}
