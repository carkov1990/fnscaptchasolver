using System.IO;
using PravoRu.Common.CaptchaSolver.Models;

namespace PravoRu.Common.CaptchaSolver.Interfaces
{
	/// <summary>
	/// Интерфейс механизма разгадывания копчи
	/// </summary>
	public interface ICaptchaSolver
	{
		/// <summary>
		/// Метод получения результата распознания капчи
		/// </summary>
		/// <param name="inputStream">Источник капчи (как правило, это WebResponse)</param>
		/// <param name="captchaSettings">Настройки капчи</param>
		/// <returns></returns>
		string SolveCaptcha(Stream inputStream, CaptchaSettings captchaSettings);
	}
}