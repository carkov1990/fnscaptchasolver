using System;
using System.Linq;

namespace PravoRu.Common.CaptchaSolver.NeuralNetwork
{
	/// <summary>
	/// Класс нейронного слоя
	/// </summary>
	public class Layer
	{
		/// <summary>
		/// Ссылка на предыдущий слой (Нужна для получения сигналов)
		/// </summary>
		private Layer _prevLayer;

		/// <summary>
		/// Ссылка на следующий слой (Нужна для продвижения вычисления дальше)
		/// </summary>
		private Layer _nextLayer;

		/// <summary>
		/// Массив нейронов
		/// </summary>
		public Neuron[] _neurons;
		
		/// <summary>
		/// Следующий слой
		/// </summary>
		public Layer NextLayer
		{
			private get { return _nextLayer; }
			set { _nextLayer = value; }
		}
		
		/// <summary>
		/// Предыдущий слой 
		/// </summary>
		public Layer PrevLayer
		{
			private get { return _prevLayer; }
			set { 
				_prevLayer = value;
				_prevLayer.NextLayer = this;
			}
		}
		
		/// <summary>
		/// Метод предсказания результата
		/// </summary>
		private void Prediction()
		{
			foreach (var neuron in _neurons)
			{
				var resultsPrevLayer = _prevLayer._neurons.Select(x => x.Signal).ToArray();
				neuron.FeedForward(resultsPrevLayer);
			}

			_nextLayer?.Prediction();
		}
		
		/// <summary>
		/// .ctor
		/// Нужен для воссоздания сети из файла json
		/// </summary>
		public Layer()
		{
		}
		
		/// <summary>
		/// .ctor
		/// </summary>
		/// <param name="neuronCount">Количество нейронов в слое</param>
		public Layer(int neuronCount) : this()
		{
			_neurons = new Neuron[neuronCount];
			for (int i = 0; i < neuronCount; i++)
			{
				_neurons[i] = new Neuron(_prevLayer?._neurons.Length ?? 0);
				_neurons[i].Number = i;
			}
		}

		/// <summary>
		/// .ctor
		/// </summary>
		/// <param name="neuronCount">Количество нейронов в слое</param>
		/// <param name="prevLayer">Предыдущий слой</param>
		public Layer(int neuronCount, Layer prevLayer) : this(neuronCount)
		{
			PrevLayer = prevLayer;
		}

		/// <summary>
		/// Метод предсказания результата по входным сигналам
		/// </summary>
		/// <param name="signals">Входные сигналы</param>
		/// <exception cref="ArgumentException">Вызывается если сигналы null или кол-во сигналов не равно кол-ву нейронов слоя</exception>
		public void Prediction(double[] signals)
		{
			if (signals == null || _neurons.Length != signals.Length)
			{
				throw new ArgumentException(nameof(signals));
			}

			for (int i = 0; i < signals.Length; i++)
			{
				_neurons[i].Signal = signals[i];
			}

			_nextLayer.Prediction();
		}
	}
}