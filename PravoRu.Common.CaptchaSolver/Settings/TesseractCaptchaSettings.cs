using PravoRu.Common.CaptchaSolver.Interfaces;

namespace PravoRu.Common.CaptchaSolver.Settings
{
	/// <summary>
	/// Настройки капчи
	/// </summary>
	public class TesseractCaptchaSettings : ITesseractCaptchaSettings
	{
		/// <summary>
		/// Путь до папки данных Тессеракт
		/// </summary>
		public string PathToTessData { get; set; }
	}
}