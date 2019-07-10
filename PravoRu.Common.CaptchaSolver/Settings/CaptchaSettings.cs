using PravoRu.Common.CaptchaSolver.Interfaces;
using PravoRu.Common.CaptchaSolver.Models;

namespace PravoRu.Common.CaptchaSolver.Settings
{
	/// <summary>
	/// Настройки механизма распознования капч
	/// </summary>
	public class CaptchaSettings : ICaptchaSettings
	{
		/// <summary>
		/// Минимальная длина капчи
		/// </summary>
		public int? MinLength { get; set; }

		/// <summary>
		/// Максимальная длина капчи
		/// </summary>
		public int? MaxLength { get; set; }

		/// <summary>
		/// Флаги
		/// </summary>
		public CaptchaFlags Flags { get; set; }
		
		/// <summary>
		/// Топология нейросети
		/// </summary>
		public ITopology Topology { get; set; }

		/// <summary>
		/// Настройки для тессеракт
		/// </summary>
		public TesseractCaptchaSettings TesseractCaptchaSettings { get; set; }

		/// <summary>
		/// Настройки антигейт
		/// </summary>
		public AntigateSettings AntigateSettings { get; set; }
	}
}