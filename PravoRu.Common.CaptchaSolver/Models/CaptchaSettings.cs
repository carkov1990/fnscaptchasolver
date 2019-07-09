namespace PravoRu.Common.CaptchaSolver.Models
{
	/// <summary>
	/// Настройки капчи
	/// </summary>
	public class CaptchaSettings
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
	}
}