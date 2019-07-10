using System;
using System.Drawing;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PravoRu.Common.CaptchaSolver.Interfaces;
using PravoRu.Common.CaptchaSolver.Models;
using PravoRu.Common.CaptchaSolver.NeuralNetwork;
using PravoRu.Common.CaptchaSolver.Settings;
using Shouldly;

namespace PravoRu.Common.CaptchaSolver.IntegrationTests
{
	[TestFixture]
	public class IoCTesseractSolverTests
	{
		private ITesseractCaptchaSolver _tesseractCaptchaSolver;
		
		[OneTimeSetUp]
		public void SetUp()
		{
			IServiceCollection serviceCollection = new ServiceCollection()
				.AddCaptchaSolvers(new CaptchaSettings()
				{
					MaxLength = 6,
					MinLength = 6,
					Flags = CaptchaFlags.HasDigits,
					TesseractCaptchaSettings = new TesseractCaptchaSettings()
					{
						PathToTessData = Path.Combine(TestContext.CurrentContext.TestDirectory, "tessdata")
					}
				});
			var provider = serviceCollection.BuildServiceProvider();
			_tesseractCaptchaSolver = provider.GetService<ITesseractCaptchaSolver>();
		}
		
		[TestCase("543299.bmp")]
		[TestCase("791148.bmp")]
		[TestCase("132167.bmp")]
		[TestCase("564715.bmp")]
		public void TesseractSolverWithIoC(string file)
		{
			//Arrange
			
			file = Path.Combine(TestContext.CurrentContext.TestDirectory, "TesseractDataSet", file);
			var expected = new FileInfo(file).Name.Substring(0, 6);
			//Act
			var actual = _tesseractCaptchaSolver.SolveCaptcha(new MemoryStream(File.ReadAllBytes(file)));

			//Assert
			actual.ShouldBe(expected);
		}
	}
}