using System;

namespace PravoRu.Common.CaptchaSolver.NeuralNetwork
{
	/// <summary>
	/// Класс нейрона
	/// </summary>
	public class Neuron
	{
		/// <summary>
		/// Входные сигналы с предыдущего слоя сети
		/// </summary>
		private double[] _inputs;
		
		/// <summary>
		/// Веса связей
		/// </summary>
		public double[] Weights;

		/// <summary>
		/// Выходной сигнал нейрона
		/// </summary>
		public double Signal = 0.0;
		
		/// <summary>
		/// Номер нейрона в слое
		/// </summary>
		public int Number = 0;
		
		/// <summary>
		/// Метод вычисления сигмоидной функции
		/// </summary>
		/// <param name="x">Аргумент x</param>
		/// <returns>Результат 1.0 / (1.0 + Math.Pow(Math.E, -x))</returns>
		private double Sigmoid(double x)
		{
			var result = 1.0 / (1.0 + Math.Pow(Math.E, -x));
			return result;
		}

		/// <summary>
		/// Метод вычисления производной сигмоидной функции
		/// </summary>
		/// <param name="x">Аргумент</param>
		/// <returns>Sigmoid(x)`</returns>
		private double SigmoidDx(double x)
		{
			var sigmoid = Sigmoid(x);
			var result = sigmoid / (1 - sigmoid);
			return result;
		}

		/// <summary>
		/// .ctor
		/// Нужен для воссоздания сети из json файла
		/// </summary>
		public Neuron()
		{
		}


		/// <summary>
		/// .ctor
		/// </summary>
		/// <param name="countWeights">Количество весов/сигналов на предыдущем слое</param>
		/// <exception cref="ArgumentException">Вызывается если кол-во сигналов меньше 1</exception>
		public Neuron(int countWeights) : this()
		{
			if (countWeights <= 0)
			{
				throw new ArgumentException(nameof(countWeights));
			}
			Weights = new double[countWeights];
			var weightRandomGenerator = new Random();
			for (int i = 0; i < countWeights; i++)
			{
				Weights[i] = Math.Round(weightRandomGenerator.NextDouble() - 0.5, 4);
			}
		}
		
		/// <summary>
		/// Метод вычисления выходного сигнала нейрона
		/// </summary>
		/// <param name="inputs">Входящие сигналы</param>
		public void FeedForward(double[] inputs)
		{
			_inputs = inputs;
			var sum = 0.0;
			for(int i = 0; i < inputs.Length; i++)
			{
				sum += inputs[i] * Weights[i];
			}
			
			Signal = Sigmoid(sum);
		}

		/// <summary>
		/// Метод обучения нейрона/ перераспределения весов
		/// </summary>
		/// <param name="error">Отклонение от ожидаемого значения</param>
		/// <param name="learningRate">Коэффициент обучения</param>
		public void Learn(double error, double learningRate)
		{
			var delta = error * SigmoidDx(Signal);

			for(int i = 0; i < Weights.Length; i++)
			{
				var weight = Weights[i];
				var input = _inputs[i];

				var newWeigth = weight - input * delta * learningRate;
				Weights[i] = newWeigth;
			}
		}
	}
}