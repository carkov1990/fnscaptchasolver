using PravoRu.Common.CaptchaSolver.Models;

namespace PravoRu.Common.CaptchaSolver.Settings
{
	/// <summary>
	/// Класс топологии сети
	/// </summary>
	public class Topology : ITopology
	{
		/// <summary>
		/// Количество слоев
		/// </summary>
		public int LayerCount { get; set; }
		
		/// <summary>
		/// Количество нейронов в каждом слое
		/// </summary>
		public int[] NeuronsCountByLayer { get; set; }
		
		/// <summary>
		/// Коэффициент обучения
		/// </summary>
		public double LearningRate { get; set; }

		/// <summary>
		/// Путь до файла конфигурации нейронной сети
		/// </summary>
		public string PathToConfigFile { get; set; }
	}
}