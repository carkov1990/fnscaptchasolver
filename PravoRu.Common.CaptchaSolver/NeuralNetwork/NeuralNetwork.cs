using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace PravoRu.Common.CaptchaSolver.NeuralNetwork
{
	/// <summary>
	/// Класс нейронной сети
	/// </summary>
	public class NeuralNetwork
	{
		/// <summary>
		/// Топология сети
		/// </summary>
		private ITopology _topology;
		
		/// <summary>
		/// Слои нейронной сети
		/// </summary>
		public Layer[] Layers;

		/// <summary>
		/// .ctor
		/// </summary>
		public NeuralNetwork()
		{
		}
		
		/// <summary>
		/// .ctor
		/// </summary>
		/// <param name="topology">Топология сети</param>
		public NeuralNetwork(ITopology topology) : this(topology.LayerCount, topology.NeuronsCountByLayer)
		{
		}

		/// <summary>
		/// .ctor
		/// </summary>
		/// <param name="layerCount">Количество слоев сети</param>
		/// <param name="neuronLayerCount">Количество нейронов в каждом слое</param>
		/// <exception cref="ArgumentException"></exception>
		private NeuralNetwork(int layerCount, params int[] neuronLayerCount)
		{
			if (layerCount < 2)
			{
				throw new ArgumentException(nameof(layerCount));
			}
			if (neuronLayerCount.Length < 2)
			{
				throw new ArgumentException(nameof(neuronLayerCount));
			}
			
			Layers = new Layer[layerCount];
			Layers[0] = new Layer(neuronLayerCount[0]);
			for (int i = 1; i < neuronLayerCount.Length; i++)
			{
				Layers[i] = new Layer(neuronLayerCount[i], Layers[i-1]);
			}
		}

		/// <summary>
		/// Метод инициализации нейронной сети из файла конфига
		/// </summary>
		/// <exception cref="ArgumentNullException">Вызывается когда топология или путь к файлу конфигурации не указан</exception>
		public void InitializeFromConfig()
		{
			if (_topology == null || String.IsNullOrWhiteSpace(_topology.PathToConfigFile))
			{
				throw new ArgumentNullException(nameof(_topology.PathToConfigFile));
			}
			InitializeFromConfig(File.ReadAllText(_topology.PathToConfigFile));
		}

		/// <summary>
		/// Метод инициализации нейронной сети из файла конфига
		/// </summary>
		/// <param name="jsonConfig">Json конфигурация</param>
		public void InitializeFromConfig(string jsonConfig)
		{
			Layers = JsonConvert.DeserializeObject<Layer[]>(jsonConfig);
			for (int i = 1; i < Layers.Length-1; i++)
			{
				Layers[i].NextLayer = Layers[i+1];
				Layers[i].PrevLayer = Layers[i-1];
			}

			Layers[0].NextLayer = Layers[1];
			Layers[Layers.Length-1].PrevLayer = Layers[Layers.Length-2];
		}

		/// <summary>
		/// Метод получения конфигурации сети
		/// </summary>
		/// <returns>Json конфигурацию сети</returns>
		public string GetConfigNetwork()
		{
			return JsonConvert.SerializeObject(Layers);
		}
		
		/// <summary>
		/// Метод обучения сети
		/// </summary>
		/// <param name="path">Путь к датасету</param>
		/// <param name="epoch">Количество эпох</param>
		public void Learn(string path, int epoch)
		{
			for (int i = 0; i < epoch; i++)
			{
				foreach (var file in Directory.GetFiles(path,"*.bmp"))
				{
					Bitmap input = new Bitmap(file);
					double[] inputSignals = new double[input.Width * input.Height];
					var z = 0;
					for (int x = 0; x < input.Width; x++)
					{
						for (int y = 0; y < input.Height; y++)
						{
							inputSignals[z] = input.GetPixel(x, y).B == 0 ? 1 : 0;
							z++;
						}
					}

					var inputLayer = Layers.First();
					inputLayer.Prediction(inputSignals);

					var expectedDigital = int.Parse(new FileInfo(file).Name[0].ToString());

					var resultNeurons = Layers.Last()._neurons;
					for (int j = 0; j < resultNeurons.Length; j++)
					{
						var expectResult = j == expectedDigital ? 1 : 0;
						var resultNeuron = resultNeurons[j].Signal;
						var difference = resultNeuron - expectResult;
						resultNeurons[j].Learn(difference, 0.1);
					}
				}
			}
		}

		/// <summary>
		/// Метод вычисления результата
		/// </summary>
		/// <param name="inputs">Входящие сигналы</param>
		/// <returns>Цифра, которую удалось распознать</returns>
		public int Prediction(double[] inputs)
		{
			var inputLayer = Layers.First();
			inputLayer.Prediction(inputs);
			var maxResult = Layers.Last()._neurons.Max(x => x.Signal);
			var result = Layers.Last()._neurons.First(x => x.Signal == maxResult).Number;
			return result;
		}

		/// <summary>
		/// Метод вычисления результата
		/// </summary>
		/// <param name="inputs">Входящие сигналы</param>
		/// <returns>Цифра, которую удалось распознать</returns>
		public int Prediction(double[,] inputs)
		{
			double[] inputSignals = new double[inputs.Length];
			var z = 0;
			for (int x = 0; x < inputs.GetLength(0); x++)
			{
				for (int y = 0; y < inputs.GetLength(1); y++)
				{
					inputSignals[z] = inputs[x, y];
					z++;
				}
			}

			return Prediction(inputSignals);
		}
		
		/// <summary>
		/// Метод вычисления результата
		/// </summary>
		/// <param name="inputs">Входящие сигналы</param>
		/// <returns>Цифра, которую удалось распознать</returns>
		public int Prediction(Bitmap input)
		{
			double[] inputSignals = new double[input.Width * input.Height];
			var z = 0;
			for (int x = 0; x < input.Width; x++)
			{
				for (int y = 0; y < input.Height; y++)
				{
					inputSignals[z] = input.GetPixel(x, y).B == 0 ? 1 : 0;
					z++;
				}
			}
			return Prediction(inputSignals);
		}
	}
}