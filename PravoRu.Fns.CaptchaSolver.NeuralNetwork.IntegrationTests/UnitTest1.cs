using System;
using System.Drawing;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using PravoRu.Fns.CaptchaSolver.NeuralNetwork;

namespace Tests
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void Test1()
		{
			var currentDirectory = Directory.GetCurrentDirectory();
			NeuralNetwork network = new NeuralNetwork(2, 25*40, 10);
			network.Learn(@"C:\Users\1\Desktop\DataSet\full",100);
			using (var sw = new StreamWriter("digitalRecognize.json", true))
			{
				sw.WriteLine(network.GetModel());
			}
			foreach (var file in Directory.GetFiles(currentDirectory,"*.bmp"))
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

				var actual = network.Prediction(inputSignals);
				var expectedDigital = int.Parse(new FileInfo(file).Name[0].ToString());
			}
		}
		
		[Test]
		public void Test12()
		{
			for (int i = 0; i < 100; i++)
			{
				var seconds = (DateTime.UtcNow - DateTime.MinValue).TotalSeconds;
				var token = new WebClient().DownloadString($"https://service.nalog.ru/static/captcha.bin?{seconds}");
				var bytes = new WebClient().DownloadData($"https://service.nalog.ru/static/captcha.bin?r={seconds}&a={token}&version=3");
				var image = Bitmap.FromStream(new MemoryStream(bytes));
				Directory.CreateDirectory("TestRecognize");
				Directory.CreateDirectory($"TestRecognize\\{i}");
				image.Save($"TestRecognize\\{i}\\{Guid.NewGuid()}.png");
				Directory.CreateDirectory("TestRecognize\\"+i);
				PravoRu.Fns.CaptchaSolver.Program.Run((Bitmap)image, Directory.GetCurrentDirectory()+ "\\TestRecognize\\"+i);
				NeuralNetwork network = new NeuralNetwork();
				network.Initialize(File.ReadAllText("digitalRecognize.json"));
				var result = "";
				foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory()+"\\TestRecognize\\"+i,"*.bmp"))
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

					var actual = network.Prediction(inputSignals);
					result += actual;
				}

				File.WriteAllBytesAsync(Directory.GetCurrentDirectory() + "\\TestRecognize\\" + i + $"\\{result}.txt",
					new byte[0]).Wait();
			}
			
		}
		
	}
}