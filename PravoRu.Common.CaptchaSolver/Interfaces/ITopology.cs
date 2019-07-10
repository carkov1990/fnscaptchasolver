namespace PravoRu.Common.CaptchaSolver.Models
{
	/// <summary>
	/// Интерфейс топологии сети
	/// </summary>
	public interface ITopology
	{
		/// <summary>
		/// Количество слоев
		/// </summary>
		int LayerCount { get; set; }
		
		/// <summary>
		/// Количество нейронов в каждом слое
		/// </summary>
		int[] NeuronsCountByLayer { get; set; }
		
		/// <summary>
		/// Коэффициент обучения
		/// </summary>
		double LearningRate { get; set; }

		/// <summary>
		/// Путь до файла конфигурации нейронной сети
		/// </summary>
		string PathToConfigFile { get; set; }
	}
}