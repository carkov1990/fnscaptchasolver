namespace PravoRu.Common.CaptchaSolver.Interfaces
{
	/// <summary>
	/// Интерфейс настроек Tesseract
	/// </summary>
	public interface ITesseractCaptchaSettings
	{
		/// <summary>
		/// Путь до папки данных Тессеракт
		/// </summary>
		string PathToTessData { get; set; }
	}
}