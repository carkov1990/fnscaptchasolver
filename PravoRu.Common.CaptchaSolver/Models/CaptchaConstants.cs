using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PravoRu.DataLake.AntiCaptcha
{
	internal static class CaptchaConstants
	{
		public static string Digits = "0123456789";

		public static string SpecSymbols = ".,<>?!'\"\\/:;!@#$%^&*()_-+=|";

		public static string LatinSmallLetters = "abcdefghigklmnopqrstuvwxyz";

		public static string LatinBigLetters = "ABCDEFGHIGKLMNOPQRSTUVWXYZ";

		public static string LatinLetters = string.Concat(LatinBigLetters, LatinSmallLetters);

		public static string CyrillicSmallLetters = "абвгдеёжзийклмнопрстуфхцчшщьыъэюя";

		public static string CyrillicBigLetters = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЬЫЪЭЮЯ";

		public static string CyrillicLetters = string.Concat(CyrillicBigLetters, CyrillicSmallLetters);
	}
}
