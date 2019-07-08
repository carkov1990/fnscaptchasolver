using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace PravoRu.Fns.CaptchaSolver.NeuralNetwork
{
	public class NeuralNetwork
	{
		public Layer[] Layers;

		public NeuralNetwork()
		{
		}

		public NeuralNetwork(int layerCount, params int[] neuronLayerCount)
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
			Layers[0] = new Layer(neuronLayerCount[0], null);
			
			for (int i = 1; i < neuronLayerCount.Length; i++)
			{
				Layers[i] = new Layer(neuronLayerCount[i], Layers[i-1]);
			}
			
			for (int i = 0; i < Layers.Length-1; i++)
			{
				Layers[i].SetNextLayer(Layers[i+1]);
			}
		}

		public void Initialize(string model)
		{
			Layers = JsonConvert.DeserializeObject<Layer[]>(model);
			for (int i = 0; i < Layers.Length-1; i++)
			{
				Layers[i].SetNextLayer(Layers[i+1]);
			}
			
			for (int i = Layers.Length-1; i > 0 ; i--)
			{
				Layers[i].SetPrevLayer(Layers[i-1]);
			}
		}

		public string GetModel()
		{
			return JsonConvert.SerializeObject(Layers);
		}
		
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
						var resultNeuron = resultNeurons[j].Result;
						var difference = resultNeuron - expectResult;
						resultNeurons[j].Learn(difference, 0.1);
					}
				}
			}
		}

		public int Prediction(double[] inputs)
		{
			var inputLayer = Layers.First();
			inputLayer.Prediction(inputs);
			var maxResult = Layers.Last()._neurons.Max(x => x.Result);
			var result = Layers.Last()._neurons.First(x => x.Result == maxResult).Number;
			return result;
		}

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