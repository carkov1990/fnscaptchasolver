using System;

namespace PravoRu.Fns.CaptchaSolver.NeuralNetwork
{
	public class Neuron
	{
		public double[] _weights;
		private double[] _inputs;

		public double Result = 0.0;
		public int Number = 0;
		
		public Neuron(int countWeights)
		{
			_weights = new double[countWeights];
			var weightRandomGenerator = new Random();
			for (int i = 0; i < countWeights; i++)
			{
				_weights[i] = Math.Round(weightRandomGenerator.NextDouble() - 0.5, 4);
			}
		}

		public Neuron(){}
		
		public void SetSignal(double signal)
		{
			Result = signal;
		}
		
		public void FeedForward(double[] inputs)
		{
			_inputs = inputs;
			var sum = 0.0;
			for(int i = 0; i < inputs.Length; i++)
			{
				sum += inputs[i] * _weights[i];
			}
			
			Result = Sigmoid(sum);
		}
		
		private double Sigmoid(double x)
		{
			var result = 1.0 / (1.0 + Math.Pow(Math.E, -x));
			return result;
		}

		private double SigmoidDx(double x)
		{
			var sigmoid = Sigmoid(x);
			var result = sigmoid / (1 - sigmoid);
			return result;
		}

		public void Learn(double error, double learningRate)
		{
			var delta = error * SigmoidDx(Result);

			for(int i = 0; i < _weights.Length; i++)
			{
				var weight = _weights[i];
				var input = _inputs[i];

				var newWeigth = weight - input * delta * learningRate;
				_weights[i] = newWeigth;
			}
		}
	}
}