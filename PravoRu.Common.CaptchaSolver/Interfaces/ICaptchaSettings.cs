using PravoRu.Common.CaptchaSolver.Models;
using PravoRu.Common.CaptchaSolver.Settings;

namespace PravoRu.Common.CaptchaSolver.Interfaces
{
	/// <summary>
	/// Интерфейс настроек капчи
	/// </summary>
	public interface ICaptchaSettings
	{
		/// <summary>
		/// Минимальная длина капчи
		/// </summary>
		int? MinLength { get; set; }

		/// <summary>
		/// Максимальная длина капчи
		/// </summary>
		int? MaxLength { get; set; }

		/// <summary>
		/// Флаги
		/// </summary>
		CaptchaFlags Flags { get; set; }
		
		/// <summary>
		/// Топология нейросети
		/// </summary>
		ITopology Topology { get; set; }

		/// <summary>
		/// Настройки для тессеракт
		/// </summary>
		TesseractCaptchaSettings TesseractCaptchaSettings { get; set; }

		/// <summary>
		/// Настройки антигейт
		/// </summary>
		AntigateSettings AntigateSettings { get; set; }
	}
}