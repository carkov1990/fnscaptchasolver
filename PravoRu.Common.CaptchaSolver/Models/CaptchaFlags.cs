using System;

namespace PravoRu.Common.CaptchaSolver.Models
{
	/// <summary>
	/// Параметры капчи
	/// </summary>
	[Flags]
	public enum CaptchaFlags
	{
		/// <summary>
		/// В капче есть цифры
		/// </summary>
		HasDigits = 1,
		/// <summary>
		/// В капче есть кириллические символы
		/// </summary>
		HasCyrillicLetters = 2,
		/// <summary>
		/// В капче есть символы латиницы
		/// </summary>
		HasLatinLetters = 4,
		/// <summary>
		/// В капче есть спец. символы
		/// </summary>
		HasSpecificSymbols = 8
	}
}