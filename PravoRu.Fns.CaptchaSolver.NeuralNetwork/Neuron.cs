using System;

namespace PravoRu.Fns.CaptchaSolver.NeuralNetwork
{
	public class Neuron
	{
		private double[] weights;
		
		public Neuron(int countWeights)
		{
			weights = new double[countWeights];
			var weightRandomGenerator = new Random();
			for (int i = 0; i < countWeights; i++)
			{
				weights[i] = weightRandomGenerator.NextDouble() - 0.5;
			}
		}
	}
}