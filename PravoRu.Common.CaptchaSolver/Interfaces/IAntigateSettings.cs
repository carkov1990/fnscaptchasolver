namespace PravoRu.Common.CaptchaSolver.Interfaces
{
	/// <summary>
	/// Интерфейс настроек antigate.com
	/// </summary>
	public interface IAntigateSettings
	{
		/// <summary>
		/// 32-символьный токен для сайта antigate.com
		/// </summary>
		string AntigateApiKey { get; set; }	
	}
}