using System;
using System.Drawing;
using System.IO;
using PravoRu.Common.CaptchaSolver.Interfaces;
using PravoRu.Common.CaptchaSolver.Models;
using PravoRu.DataLake.AntiCaptcha;
using Tesseract;

namespace PravoRu.Common.CaptchaSolver.Solvers
{
	/// <summary>
	/// Решатель капчи, использующий библиотеку Tesseract
	/// </summary>
	public class TesseractCaptchaSolver: ITesseractCaptchaSolver
	{
		/// <summary>
		/// Решает капчу
		/// </summary>
		/// <param name="inputStream">Входящий поток, содержащий картинку с капчей</param>
		/// <param name="captchaSettings">Настройки капчи</param>
		/// <returns>Текст решенной капчи</returns>
		public string SolveCaptcha(Stream inputStream, CaptchaSettings captchaSettings)
		{
			Image img = Image.FromStream(inputStream);

			Bitmap bmpImage = img as Bitmap;

			if (bmpImage == null)
				throw new NotSupportedException("Image is not bitmap");

			return SolveCaptcha(bmpImage, captchaSettings);
		}


		/// <summary>
		/// Решает капчу
		/// </summary>
		/// <param name="image">Картика с капчей</param>
		/// <param name="parameters">Настройки капчи</param>
		/// <returns>Текст решенной капчи</returns>
		public string SolveCaptcha(Bitmap image, CaptchaSettings parameters)
		{
			using (var tessEngine = new TesseractEngine("tessdata", "eng"))
			{
				string whitelist = GetWhiteList(parameters.Flags);
				string blacklist = GetBlackList(parameters.Flags);

				if (!string.IsNullOrEmpty(whitelist))
					tessEngine.SetVariable("tessedit_char_whitelist", whitelist);
				if (!string.IsNullOrEmpty(blacklist))
					tessEngine.SetVariable("tessedit_char_blacklist", blacklist);

				tessEngine.SetVariable("unrecognised_char", "*");

				using (var page = tessEngine.Process(image, PageSegMode.SingleWord))
				{
					return (page.GetText() ?? string.Empty).Trim();
				}
			}
		}

		private string GetBlackList(CaptchaFlags captchaFlags)
		{
			string blacklist = string.Empty;
			if (!captchaFlags.HasFlag(CaptchaFlags.HasDigits))
				blacklist += CaptchaConstants.Digits;
			if (!captchaFlags.HasFlag(CaptchaFlags.HasCyrillicLetters))
				blacklist += CaptchaConstants.CyrillicLetters;
			if (!captchaFlags.HasFlag(CaptchaFlags.HasLatinLetters))
				blacklist += CaptchaConstants.LatinLetters;
			if (!captchaFlags.HasFlag(CaptchaFlags.HasSpecificSymbols))
				blacklist += CaptchaConstants.SpecSymbols;

			return blacklist;
		}

		private string GetWhiteList(CaptchaFlags captchaFlags)
		{
			string whitelist = string.Empty;
			if (captchaFlags.HasFlag(CaptchaFlags.HasDigits))
				whitelist += CaptchaConstants.Digits;
			if (captchaFlags.HasFlag(CaptchaFlags.HasCyrillicLetters))
				whitelist += CaptchaConstants.CyrillicLetters;
			if (captchaFlags.HasFlag(CaptchaFlags.HasLatinLetters))
				whitelist += CaptchaConstants.LatinLetters;
			if (captchaFlags.HasFlag(CaptchaFlags.HasSpecificSymbols))
				whitelist += CaptchaConstants.SpecSymbols;

			return whitelist;
		}
	}
}