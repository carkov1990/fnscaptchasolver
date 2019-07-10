using System;
using Microsoft.Extensions.DependencyInjection;
using PravoRu.Common.CaptchaSolver.Interfaces;
using PravoRu.Common.CaptchaSolver.Models;
using PravoRu.Common.CaptchaSolver.NeuralNetwork;
using PravoRu.Common.CaptchaSolver.Settings;
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
		/// <param name="settings">Настройки для распознователей капчи</param>
		public static IServiceCollection AddCaptchaSolvers(this IServiceCollection collection,
			ICaptchaSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			collection.AddSingleton<INeuralNetwork>(provider=>new NeuralNetwork.NeuralNetwork(settings.Topology));
			collection.AddSingleton<INeuralCaptchaSolver>(provider =>
			{
				var network = provider.GetService<INeuralNetwork>();
				return new NeuralCaptchaSolver(network, settings);
			});

#if NET462
			collection.AddSingleton<IAntigateCaptchaSolver>(provider =>
				new AntigateCaptchaSolver(settings.AntigateSettings, settings));
			collection.AddSingleton<ITesseractCaptchaSolver>(provider =>
				new TesseractCaptchaSolver(settings.TesseractCaptchaSettings, settings));
#endif

			return collection;
		}
	}
}