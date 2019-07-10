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

namespace PravoRu.Common.CaptchaSolver.Core.IntegrationTests
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
					Flags = CaptchaFlags.HasDigits
				});
			var provider = serviceCollection.BuildServiceProvider();
			_tesseractCaptchaSolver = provider.GetService<ITesseractCaptchaSolver>();
		}
		
		[Test]
		public void TesseractSolverWithIoC()
		{
			//Act
			_tesseractCaptchaSolver.ShouldBeNull();
		}
	}
}