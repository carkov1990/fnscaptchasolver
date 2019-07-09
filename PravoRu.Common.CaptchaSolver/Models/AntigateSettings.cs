using PravoRu.Common.ServiceDiscovery.Configuration;

namespace PravoRu.Common.CaptchaSolver.Models
{
	/// <summary>
	/// Настройки доступа к Antigate.com
	/// </summary>
	public class AntigateSettings : ConsuledSettings
	{
		/// <summary>
		/// 32-символьный токен для сайта antigate.com
		/// </summary>
		[SettingPath("DataLake/AntiCaptcha")]
		[SettingsDescription("32-символьный токен для сайта antigate.com (пример: 1a6bb3807a6eee52a19bc1ec4a7be283)")]
		public string AntigateApiKey { get; set; }
	}
}