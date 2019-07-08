using System;
using System.Collections.Generic;
using System.Linq;

namespace PravoRu.Fns.CaptchaSolver.NeuralNetwork
{
	public class Layer
	{
		private Layer _prevLayer;
		private Layer _nextLayer;
		
		public Neuron[] _neurons;

		
		public Layer(int neuronCount, Layer prevLayer)
		{
			_prevLayer = prevLayer;
			_neurons = new Neuron[neuronCount];
			for (int i = 0; i < neuronCount; i++)
			{
				_neurons[i] = new Neuron(_prevLayer?._neurons.Length ?? 0);
				_neurons[i].Number = i;
			}
		}

		public Layer()
		{
		}

		public void SetNextLayer(Layer nextLayer)
		{
			_nextLayer = nextLayer;
			
		}
		
		public void SetPrevLayer(Layer pervLayer)
		{
			_prevLayer = pervLayer;
			
		}

		public void Prediction(double[] signals)
		{
			if (_neurons.Length != signals.Length)
			{
				throw new ArgumentException(nameof(signals));
			}

			for (int i = 0; i < signals.Length; i++)
			{
				_neurons[i].SetSignal(signals[i]);
			}
			
			_nextLayer.Prediction();
		}

		private void Prediction()
		{
			foreach (var neuron in _neurons)
			{
				var resultsPrevLayer = _prevLayer._neurons.Select(x => x.Result).ToArray();
				neuron.FeedForward(resultsPrevLayer);
			}
			_nextLayer?.Prediction();
		}
	}
}