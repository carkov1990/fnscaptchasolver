using System.Drawing;
using System.IO;
using NUnit.Framework;
using PravoRu.Common.CaptchaSolver.Interfaces;
using PravoRu.Common.CaptchaSolver.Models;
using PravoRu.Common.CaptchaSolver.Settings;
using PravoRu.Common.CaptchaSolver.Solvers;
using Shouldly;

namespace PravoRu.Common.CaptchaSolver.IntegrationTests
{
	[TestFixture]
	public class TesseractSolverTests
	{
		private TesseractCaptchaSolver _tessEngine;
		private ICaptchaSettings _captchaSettings;

		[OneTimeSetUp]
		public void Setup()
		{
			_tessEngine = new TesseractCaptchaSolver(
				new TesseractCaptchaSettings()
					{PathToTessData = TestContext.CurrentContext.TestDirectory + "\\tessdata"},
				new CaptchaSettings()
				{
					MaxLength = 6,
					MinLength = 6,
					Flags = CaptchaFlags.HasDigits
				});
		}

		[TestCase("543299.bmp")]
		[TestCase("791148.bmp")]
		[TestCase("132167.bmp")]
		[TestCase("564715.bmp")]
		public void Recognize(string file)
		{
			//Arrange
			file = TestContext.CurrentContext.TestDirectory + "\\TesseractDataSet\\" + file;
			//Act
			var actual = _tessEngine.SolveCaptcha(Bitmap.FromFile(file) as Bitmap);

			//Assert
			var expected = new FileInfo(file).Name.Substring(0, 6);

			actual.ShouldBe(expected);
		}
	}
}