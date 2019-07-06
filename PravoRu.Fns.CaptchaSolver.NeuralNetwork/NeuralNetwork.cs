using System.Collections.Generic;

namespace PravoRu.Fns.CaptchaSolver.NeuralNetwork
{
	public class NeuralNetwork
	{
		private readonly List<Layer> _layers;

		public NeuralNetwork(int layerCount,params int[] neuronLayerCount)
		{
			_layers = new List<Layer>(layerCount);
			foreach (var layer in _layers)
			{
				
			}
		}

	}
}