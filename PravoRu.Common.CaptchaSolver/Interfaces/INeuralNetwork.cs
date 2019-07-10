using System.Drawing;

namespace PravoRu.Common.CaptchaSolver.Interfaces
{
	/// <summary>
	/// Интерфейс реализации нейронной сети
	/// </summary>
	public interface INeuralNetwork
	{
		/// <summary>
		/// Метод инициализации нейронной сети из файла конфига
		/// </summary>
		void InitializeFromConfig();

		/// <summary>
		/// Метод инициализации нейронной сети из файла конфига
		/// </summary>
		/// <param name="jsonConfig">Json конфигурация</param>
		void InitializeFromConfig(string jsonConfig);

		/// <summary>
		/// Метод получения конфигурации сети
		/// </summary>
		/// <returns>Конфигурация сети</returns>
		string GetConfigNetwork();

		/// <summary>
		/// Метод обучения сети
		/// </summary>
		/// <param name="path">Путь к датасету</param>
		/// <param name="epoch">Количество эпох</param>
		void Learn(string path, int epoch);

		/// <summary>
		/// Метод вычисления результата
		/// </summary>
		/// <param name="inputs">Входящие сигналы</param>
		/// <returns>Цифра, которую удалось распознать</returns>
		int Prediction(double[] inputs);

		/// <summary>
		/// Метод вычисления результата
		/// </summary>
		/// <param name="bitmap">Нормализованное изображение</param>
		/// <returns>Цифра, которую удалось распознать</returns>
		int Prediction(Bitmap bitmap);
	}
}