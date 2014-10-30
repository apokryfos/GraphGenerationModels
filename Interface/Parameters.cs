using System; using GraphFramework.Interfaces; using GraphFramework; using GraphFramework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using DataStructures;

namespace GraphGenerationModelsSample.Interface {

	public enum ParameterType { INT, PROBABILITY, DOUBLE, BOOL }


	/// <summary>
	/// Graph generation model parameter descriptor. Could be used for presentation purposes.
	/// </summary>
	public class ParameterDescriptor {
		public static ParameterDescriptor CreateProbabilityParameterDescriptor(string name) {
			return new ParameterDescriptor(name, ParameterType.PROBABILITY);
		}
		public static ParameterDescriptor CreateProbabilityParameterDescriptor(string name, double defaultValue) {
			return new ParameterDescriptor(name, ParameterType.PROBABILITY, 0.0, 1.0, defaultValue);
		}

		public static ParameterDescriptor CreateParameterDescriptor(string name, ParameterType type, double minValue, double maxValue) {
			return new ParameterDescriptor(name, type, minValue, maxValue);
		}
		public static ParameterDescriptor CreateParameterDescriptor(string name, ParameterType type, double minValue, double maxValue, double defaultValue) {
			return new ParameterDescriptor(name, type, minValue, maxValue, defaultValue);
		}
		public static ParameterDescriptor CreateParameterDescriptor(string name, ParameterType type) {
			return new ParameterDescriptor(name, type);
		}

		private string name;
		private ParameterType type;
		private double minValue, maxValue, defaultValue;

		private ParameterDescriptor() { }
		private ParameterDescriptor(string name, ParameterType type) {
			this.name = name;
			this.type = type;

			minValue = 0.0;
			defaultValue = double.NaN;

			if (type == ParameterType.DOUBLE) {
				maxValue = double.PositiveInfinity;
			} else if (type == ParameterType.INT) {
				maxValue = int.MaxValue;
			} else {
				maxValue = 1.0;
				defaultValue = 1.0;
			}
		}

		private ParameterDescriptor(string name, ParameterType type, double minValue, double maxValue) {
			this.name = name;
			this.type = type;

			this.minValue = minValue;
			this.maxValue = maxValue;
			defaultValue = double.NaN;

			if (type == ParameterType.PROBABILITY) {
				this.minValue = Math.Max(0.0, minValue);
				this.maxValue = Math.Min(1.0, maxValue);
				defaultValue = 1.0;
			}
		}
		private ParameterDescriptor(string name, ParameterType type, double minValue, double maxValue, double defaultValue)
			: this(name, type, minValue, maxValue) {
			this.defaultValue = defaultValue;
			this.defaultValue = Math.Min(Math.Max(minValue, defaultValue), maxValue);
		}

		public string Name { get { return name; } }
		public ParameterType Type { get { return type; } }
		public double Min { get { return minValue; } }
		public double Max { get { return maxValue; } }
		public double Default { get { return defaultValue; } }
	}
}
