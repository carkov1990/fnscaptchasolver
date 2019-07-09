using System;
using System.Drawing;
using System.IO;
using Akumu.Antigate;
using NLog;
using PravoRu.Common.CaptchaSolver.Interfaces;
using PravoRu.Common.CaptchaSolver.Models;

namespace PravoRu.Common.CaptchaSolver.Solvers
{
	/// <summary>
	/// Решатель капчи, использующий платный сервис anti-captcha.com
	/// </summary>
	public class AntigateCaptchaSolver : IAntigateCaptchaSolver
	{
		private readonly string _apiKey;
		private ILogger _logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="settings">Настройки доступа к Antigate.com</param>
		public AntigateCaptchaSolver(AntigateSettings settings)
		{
			_apiKey = settings.AntigateApiKey;
		}
		
		/// <summary>
		/// .ctor
		/// </summary>
		public AntigateCaptchaSolver()
		{
			_apiKey = new AntigateSettings().AntigateApiKey;
		}

		/// <summary>
		/// Решает капчу
		/// </summary>
		/// <param name="imageData">Входящий поток, содержащий картинку с капчей</param>
		/// <param name="captchaSettings">Настройки капчи</param>
		/// <returns>Текст решенной капчи</returns>
		public string SolveCaptcha(Stream imageData, CaptchaSettings captchaSettings)
		{
			var image = Image.FromStream(imageData);

			try
			{
				// инициализируем сервис обработки капч с помощью API key
				Akumu.Antigate.AntiCaptcha antiCaptcha = new Akumu.Antigate.AntiCaptcha(_apiKey);

				// минимальная длина текста на капче
				if (captchaSettings.MinLength.HasValue)
					antiCaptcha.Parameters.Set("min_len", captchaSettings.MinLength.ToString());
				// максимальная длина текста на капче
				if (captchaSettings.MaxLength.HasValue)
					antiCaptcha.Parameters.Set("max_len", captchaSettings.MaxLength.ToString());
				// не обязательно показывать капчу русскому работнику
				string needRussianBot = captchaSettings.Flags.HasFlag(CaptchaFlags.HasCyrillicLetters) ? "1" : "0";
				antiCaptcha.Parameters.Set("is_russian", needRussianBot);
				// в капче только цифры
				if (captchaSettings.Flags == CaptchaFlags.HasDigits)
					antiCaptcha.Parameters.Set("numeric", "1");
				
				antiCaptcha.CheckDelay = 1000;
				antiCaptcha.CheckRetryCount = 60;
				antiCaptcha.SlotRetry = 10;
				antiCaptcha.SlotRetryDelay = 1000;

				// Отправляем изображение и ждем решения
				string answer = antiCaptcha.GetAnswer(image);

				return answer;
			}
			catch (AntigateErrorException aee)
			{
				_logger.Error(aee, "С сервиса anti-captcha.com возвращена ошибка");
			}
			catch (Exception e)
			{
				_logger.Error(e, "Общая ошибка при попытке решить капчу");
			}
			return null;
		}
	}
}