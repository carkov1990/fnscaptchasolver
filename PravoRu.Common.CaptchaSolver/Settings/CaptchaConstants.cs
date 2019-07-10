using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PravoRu.DataLake.AntiCaptcha
{
	/// <summary>
	/// Константы для капч
	/// </summary>
	internal static class CaptchaConstants
	{
		/// <summary>
		/// Цифры
		/// </summary>
		public static string Digits = "0123456789";

		/// <summary>
		/// Специальные символы
		/// </summary>
		public static string SpecSymbols = ".,<>?!'\"\\/:;!@#$%^&*()_-+=|";

		/// <summary>
		/// Строчные латинские буквы
		/// </summary>
		public static string LatinSmallLetters = "abcdefghigklmnopqrstuvwxyz";

		/// <summary>
		/// Прописные латинские буквы
		/// </summary>
		public static string LatinBigLetters = "ABCDEFGHIGKLMNOPQRSTUVWXYZ";

		/// <summary>
		/// Латинские буквы
		/// </summary>
		public static string LatinLetters = string.Concat(LatinBigLetters, LatinSmallLetters);

		/// <summary>
		/// Строчные кирилические буквы
		/// </summary>
		public static string CyrillicSmallLetters = "абвгдеёжзийклмнопрстуфхцчшщьыъэюя";

		/// <summary>
		/// Прописные кирилические буквы
		/// </summary>
		public static string CyrillicBigLetters = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЬЫЪЭЮЯ";

		/// <summary>
		/// Кирилические буквы
		/// </summary>
		public static string CyrillicLetters = string.Concat(CyrillicBigLetters, CyrillicSmallLetters);
	}
}
