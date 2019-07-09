using System;
using Microsoft.Extensions.DependencyInjection;
using PravoRu.Common.CaptchaSolver.Interfaces;
using PravoRu.Common.CaptchaSolver.NeuralNetwork;
using PravoRu.Common.CaptchaSolver.Solvers;

namespace PravoRu.Common.CaptchaSolver
{
	/// <summary>
	/// Класс расширения для распознавания капчи
	/// </summary>
	public static class IoCInstaller
	{
		/// <summary>
		/// Метод формирования DI контейнера
		/// </summary>
		/// <param name="collection">IServiceCollection</param>
		/// <param name="topology">Топология нейронной сети</param>
		public static void UseCaptchaSolver(this IServiceCollection collection, Topology topology)
		{
			collection.AddSingleton<ITopology>(provider => topology);
			collection.AddSingleton<IAntigateCaptchaSolver, AntigateCaptchaSolver>();
			collection.AddSingleton<ITesseractCaptchaSolver, TesseractCaptchaSolver>();
			collection.AddSingleton<INeuralCaptchaSolver, NeuralCaptchaSolver>();
		}
	}
}