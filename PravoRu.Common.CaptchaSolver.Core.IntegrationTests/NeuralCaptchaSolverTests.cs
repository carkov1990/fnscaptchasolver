using System.Drawing;
using System.IO;
using NUnit.Framework;
using PravoRu.Common.CaptchaSolver.Interfaces;
using PravoRu.Common.CaptchaSolver.Models;
using PravoRu.Common.CaptchaSolver.Settings;
using PravoRu.Common.CaptchaSolver.Solvers;
using Shouldly;

namespace PravoRu.Common.CaptchaSolver.Core.IntegrationTests
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class NeuralCaptchaSolverTests
	{
		private NeuralCaptchaSolver _solver;
		private ICaptchaSettings _captchaSettings;

		[OneTimeSetUp]
		public void Setup()
		{
			var topology = new Topology()
			{
				PathToConfigFile = Path.Combine(TestContext.CurrentContext.TestDirectory,
					"digitalRecognize.json")
			};
			_solver = new NeuralCaptchaSolver(
				new NeuralNetwork.NeuralNetwork(topology),
				new CaptchaSettings()
				{
					MaxLength = 6,
					MinLength = 6,
					Flags = CaptchaFlags.HasDigits,
					Topology = topology
				});
		}

		[TestCase("038722.bmp")]
		[TestCase("930993.bmp")]
		[TestCase("168539.bmp")]
		[TestCase("880728.png")]
		[TestCase("987827.png")]
		[TestCase("083189.png")]
		[TestCase("532667.png")]
		[TestCase("190787.png")]
		[TestCase("903873.png")]
		[TestCase("810650.png")]
		[TestCase("933197.png")]
		[TestCase("926300.png")]
		[TestCase("749057.png")]
		[TestCase("672716.png")]
		[TestCase("668822.png")]
		
		public void Recognize(string file) 
		{
			//Arrange
			file = Path.Combine(TestContext.CurrentContext.TestDirectory, "NeuralNetworkDataSet", file);
			var expected = new FileInfo(file).Name.Substring(0, 6);
			//Act
			var actual = _solver.SolveCaptcha(new MemoryStream(File.ReadAllBytes(file)));

			//Assert
			actual.ShouldBe(expected);
		}
		
		[Explicit("Данные тесты для улучшения работы парсера капчи. Они не проходят - нужно улучшить работу парсера")]
		[TestCase("686093.png")]
		public void ForBetter(string file) 
		{
			//Arrange
			file = Path.Combine(TestContext.CurrentContext.TestDirectory, "NeuralNetworkDataSet", file);
			var expected = new FileInfo(file).Name.Substring(0, 6);
			//Act
			var actual = _solver.SolveCaptcha(new MemoryStream(File.ReadAllBytes(file)));

			//Assert
			actual.ShouldBe(expected);
		}
	}
}