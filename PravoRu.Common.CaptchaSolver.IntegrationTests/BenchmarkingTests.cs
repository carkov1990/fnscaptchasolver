using System;
using System.Drawing;
using System.IO;
using System.Net;
using NUnit.Framework;
using PravoRu.Common.CaptchaSolver.Interfaces;
using PravoRu.Common.CaptchaSolver.Models;
using PravoRu.Common.CaptchaSolver.NeuralNetwork;
using PravoRu.Common.CaptchaSolver.Settings;
using PravoRu.Common.CaptchaSolver.Solvers;

namespace PravoRu.Common.CaptchaSolver.IntegrationTests
{
	[Explicit("Данный тест для сравнительного анализа работы Tesseract vs NeuralNetwork")]
	[TestFixture]
	public class BenchmarkingTests
	{
		string folder;
		TesseractCaptchaSolver tessEngine;
		NeuralCaptchaSolver neuralCaptchaSolver;
		ICaptchaSettings _captchaSettings;

		[OneTimeSetUp]
		public void Setup()
		{
			folder = TestContext.CurrentContext.TestDirectory + "\\TestRecognize";
			Directory.CreateDirectory(folder);
			var captchaSettings = new CaptchaSettings()
			{
				MaxLength = 6,
				MinLength = 6,
				Flags = CaptchaFlags.HasDigits
			};
			tessEngine = new TesseractCaptchaSolver(new TesseractCaptchaSettings()
			{
				PathToTessData = TestContext.CurrentContext.TestDirectory + "\\tessdata"
			}, captchaSettings);
			neuralCaptchaSolver = new NeuralCaptchaSolver(new NeuralNetwork.NeuralNetwork(new Topology()
				{
					PathToConfigFile = TestContext.CurrentContext.TestDirectory + "\\digitalRecognize.json"
				}),
				captchaSettings);
			using (var sw = new StreamWriter($"{folder}\\results.csv",
				true))
			{
				sw.WriteLine($@"NamePictureTesseract;TesseractResult;NamePictureNeuralNetwork;NeuralResult");
			}
		}

		[Test]
		public void TestTesseractNeural()
		{
			for (int i = 0; i < 100; i++)
			{
				var seconds = (DateTime.UtcNow - DateTime.MinValue).TotalSeconds;
				var token = new WebClient().DownloadString($"https://service.nalog.ru/static/captcha.bin?{seconds}");
				var bytes3 =
					new WebClient().DownloadData(
						$"https://service.nalog.ru/static/captcha.bin?r={seconds}&a={token}&version=3");
				var bytes1 =
					new WebClient().DownloadData(
						$"https://service.nalog.ru/static/captcha.bin?r={seconds}&a={token}&version=1");
				var image3 = Bitmap.FromStream(new MemoryStream(bytes3));
				var image1 = Bitmap.FromStream(new MemoryStream(bytes1));
				var namePicture = (1000000 + i) + ".png";
				image1.Save($"{folder}\\1{namePicture}");
				image3.Save($"{folder}\\3{namePicture}");
				var tesseractResult = tessEngine.SolveCaptcha(new MemoryStream(bytes1));
				var neuralResult = neuralCaptchaSolver.SolveCaptcha(new MemoryStream(bytes3));
				using (var sw = new StreamWriter($"{folder}\\results.csv",
					true))
				{
					sw.WriteLine($@"1{namePicture};{tesseractResult};3{namePicture};{neuralResult}");
				}
			}
		}
	}
}