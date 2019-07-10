using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using PravoRu.Common.CaptchaSolver.Interfaces;
using PravoRu.Common.CaptchaSolver.Models;

namespace PravoRu.Common.CaptchaSolver.Solvers
{
	/// <summary>
	/// Класс механизма распознания капчи на основе нейронной сети
	/// </summary>
	public class NeuralCaptchaSolver : INeuralCaptchaSolver
	{
		private readonly ICaptchaSettings _settings;
		private INeuralNetwork _network;
		
		/// <summary>
		/// .ctor
		/// </summary>
		public NeuralCaptchaSolver(INeuralNetwork network, ICaptchaSettings settings)
		{
			_settings = settings?? throw new ArgumentNullException(nameof(settings));
			_network = network ?? throw new ArgumentNullException(nameof(network));
			if (!string.IsNullOrWhiteSpace(settings.Topology.PathToConfigFile))
			{
				_network.InitializeFromConfig();
			}
		}

		/// <inheritdoc cref="ICaptchaSolver.SolveCaptcha"/>
		public string SolveCaptcha(Stream inputStream)
		{
			//Получаем картинки в которых содержатся только цифры
			var bitmaps = CutCaptcha(Image.FromStream(inputStream) as Bitmap);
			var result = "";
			//В цикле пробегаемся по картинкам и отдаем на распознание нейронной сети
			foreach (var bitmap in bitmaps)
			{
				result+=_network.Prediction(bitmap);
			}

			return result;
		}

		private Bitmap[] CutCaptcha(Bitmap bitmap)
		{
			if (bitmap == null)
			{
				throw new ArgumentException(nameof(bitmap));
			}
			//Делаем капчу 2х цветной (черный/белый)
			SetBlackWhiteBitmap(bitmap);
			
			//Иногда несколько цифр соединяются одной фигурой из за длинных черт. Удалим их(попробуем)
			CutLittleLinesX(bitmap,2);
			CutLittleLinesY(bitmap,2);
			
			//Разрезаем капчу на отдельные части, содержащие в себе только цифры и приведенные к стандарту 25x40
			return GetCuttedBitmaps(bitmap);
		}
		
		/// <summary>
		/// Метод преобразования изображения в ч/б 
		/// </summary>
		/// <param name="bitmap">Исходное изображение</param>
		private void SetBlackWhiteBitmap(Bitmap bitmap)
		{
			for (int x = 0; x < bitmap.Width; x++)
			{
				for (int y = 0; y < bitmap.Height; y++)
				{
					var pixel = bitmap.GetPixel(x, y);
					//Получаем пиксель и смотрим приближение к какому цвету
					if (pixel.R > 127 && pixel.G > 127 && pixel.B > 127)
					{
						bitmap.SetPixel(x, y, Color.White);
					}
					else
					{
						bitmap.SetPixel(x, y, Color.Black);
					}
				}
			}
		}
		
		/// <summary>
		/// Получение нормализованных картинок 
		/// </summary>
		/// <param name="bitmap">Исходное изображение</param>
		private Bitmap[] GetCuttedBitmaps(Bitmap bitmap)
		{
			//Получаем картинки(логика отбора картинок по площади заливки)
			var cuttedBitmaps = GetBitmapsBySquare(bitmap);
			//3 раза проходимся по массиву с картинками, это сделано для того, чтобы разделить сразу не разделенные цифры
			for (int i = 0; i < 3; i++)
			{
				var resultBitmaps = new List<Bitmap>(6);
				if (cuttedBitmaps.Count < 6)
				{
					foreach (var cuttedBitmap in cuttedBitmaps)
					{
						//Если ширина картинки боше 35 px, то скорее всего в этой картинке 2 цифры
						if (cuttedBitmap.Width > 35)
						{
							//пробуем разделить картинку
							var resized = GetBitmapsBySquare(cuttedBitmap);
							foreach (var bitmap1 in resized)
							{
								resultBitmaps.Add(bitmap1);
							}
						}
						else
						{
							resultBitmaps.Add(cuttedBitmap);
						}
					}
					cuttedBitmaps = resultBitmaps;
				}
			}

			//Попробуем просто разделить пополам те картинки, которые шире нужного размера
			if (cuttedBitmaps.Count < 6)
			{
				var resultBitmaps = new List<Bitmap>(6);
					foreach (var cuttedBitmap in cuttedBitmaps)
					{
						//Если ширина картинки боше 35 px, то скорее всего в этой картинке 2 цифры
						if (cuttedBitmap.Width > 35)
						{
							//пробуем разделить картинку
							var partCount = cuttedBitmap.Width / 35;
							for (int i = 0; i <= partCount; i++)
							{
								var width = cuttedBitmap.Width / (partCount + 1);
								var part = CropImage(cuttedBitmap,
									new Rectangle(i*width, 0, width , cuttedBitmap.Height));
								resultBitmaps.Add(part);
							}
						}
						else
						{
							resultBitmaps.Add(cuttedBitmap);
						}
					}
					cuttedBitmaps = resultBitmaps;
			}
			return cuttedBitmaps.Select(x=>Resize(x)).ToArray();
		}
		
		//Метод изменения размера изображения
		private Bitmap Resize (Bitmap original)
		{
			Graphics g = null;
			Bitmap b = null;
			try {
				b = new Bitmap(25,40);
				g = Graphics.FromImage(b);
				g.Clear(Color.Transparent);
				g.DrawImage(original, 0, 0, 25, 40);
			}
			finally {
				if (g != null) {
					g.Dispose();
				}
			}

			return b;
		}
		
		/// <summary>
		/// Метод получения выделенных цифр методом заливки
		/// </summary>
		/// <param name="bitmap">Исходное изображение</param>
		/// <returns>Список картинок</returns>
		private List<Bitmap> GetBitmapsBySquare(Bitmap bitmap)
		{
			//Извлеченные картинки
			var bitmaps = new List<Bitmap>(10);
			//Уже использованные точки
			var usedPointArray = new List<Point>();
			for (int x = 1; x < bitmap.Width; x++)
			{
				for (int y = 1; y < bitmap.Height; y++)
				{
					//Если данная точка уже была ранее использована, то просто бежим дальше
					if (usedPointArray.Contains(new Point(x, y)))
					{
						continue;
					}

					//если текущий пиксель черный, то нужно получить залитую фигуру 
					if (bitmap.GetPixel(x, y).B == 0)
					{
						//Получаем массив залитых координат
						List<Point> pointArray = GetShape(bitmap, x, y);
						//Добавляем точки в массив использованных координат, чтобы не проверять их по n раз
						usedPointArray.AddRange(pointArray);
						//Если фигура площадью меньше 120 точек, то next
						if (pointArray.Count < 120)
						{
							continue;
						}
						
						//Предположим, что мы получили цифру
						Bitmap digit = new Bitmap(bitmap.Width, bitmap.Height);
						//Зальем ее белым цветом, а там где должна быть фигура черным
						for (int x1 = 0; x1 < digit.Width; x1++)
						{
							for (int y1 = 0; y1 < digit.Height; y1++)
							{
								if (pointArray.Contains(new Point(x1, y1)))
								{
									digit.SetPixel(x1,y1, Color.Black);
								}
								else
								{
									digit.SetPixel(x1,y1, Color.White);
								}
							}
						}
						
						//Удалим линии по ширине и высоте
						CutLittleLinesX(digit);
						CutLittleLinesY(digit);

						//Обрежим лишние пиксели у картинки 
						var cutBitmap = CutBitmap(digit);
						if (cutBitmap != null)
						{
							bitmaps.Add(cutBitmap);
						}
					}
				}
			}

			return bitmaps;
		}
		
		/// <summary>
		/// Метод обрезания тонких длинных линий
		/// </summary>
		private void CutLittleLinesX(Bitmap digit, int pixelCount = 4)
		{
			//Попробуем убрать тонкие линии меньше 4х пикселей
			var checkedPixels = new List<Point>();
			for (int xDigit = 0; xDigit < digit.Width; xDigit++)
			{
				for (int yDigit = 0; yDigit < digit.Height; yDigit++)
				{
					if (checkedPixels.Contains(new Point(xDigit, yDigit))) continue;
					var pixel = digit.GetPixel(xDigit, yDigit);
					if (pixel.B == 0)
					{
						var localPixels = new List<Point>
						{
							new Point(xDigit, yDigit)
						};
						for (int jIndex = yDigit; jIndex < digit.Height; jIndex++)
						{
							var pixelLocal = digit.GetPixel(xDigit, jIndex);
							if (pixelLocal.B == 0)
							{
								localPixels.Add(new Point(xDigit,jIndex));
							}
							else
							{
								break;
							}
						}

						if (localPixels.Count > pixelCount)
						{
							checkedPixels.AddRange(localPixels);
						}
						else
						{
							foreach (var localPixel in localPixels)
							{
								digit.SetPixel(localPixel.X, localPixel.Y, Color.White);
							}
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Метод обрезания тонких длинных линий
		/// </summary>
		private void CutLittleLinesY(Bitmap digit, int pixelCount = 4)
		{
			//Попробуем убрать тонкие линии меньше 4х пикселей
			var checkedPixels = new List<Point>();
			
			for (int yDigit = 0; yDigit < digit.Height; yDigit++)
			{
				for (int xDigit = 0; xDigit < digit.Width; xDigit++)
				{
					if (checkedPixels.Contains(new Point(xDigit, yDigit))) continue;
					var pixel = digit.GetPixel(xDigit, yDigit);
					if (pixel.B == 0)
					{
						var localPixels = new List<Point>
						{
							new Point(xDigit, yDigit)
						};
						for (int jIndex = xDigit; jIndex < digit.Width; jIndex++)
						{
							var pixelLocal = digit.GetPixel(jIndex, yDigit);
							if (pixelLocal.B == 0)
							{
								localPixels.Add(new Point(jIndex, yDigit));
							}
							else
							{
								break;
							}
						}

						if (localPixels.Count > pixelCount)
						{
							checkedPixels.AddRange(localPixels);
						}
						else
						{
							foreach (var localPixel in localPixels)
							{
								digit.SetPixel(localPixel.X, localPixel.Y, Color.White);
							}
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Метод получения списка координат залитой фигуры
		/// </summary>
		/// <param name="bitmap"></param>
		/// <param name="x">X координата начальной точки</param>
		/// <param name="y">Y координата начальной точки</param>
		/// <returns>Список координат</returns>
		private List<Point> GetShape(Bitmap bitmap, int x, int y)
		{
			List<Point> points =new List<Point>(300);
			points.Add(new Point(x,y));
			for (int i = 0; i < points.Count; i++)
			{
				var point = points[i];
				var upPoint = new Point(point.X,point.Y-1);
				if (!points.Contains(upPoint))
				{
					if (upPoint.Y >= 0 && bitmap.GetPixel(upPoint.X, upPoint.Y).B == 0)
					{
						points.Add(upPoint);
					}
				}
				
				
				var rightPoint = new Point(point.X+1,point.Y);
				if (!points.Contains(rightPoint))
				{
					if (rightPoint.X < bitmap.Width && bitmap.GetPixel(rightPoint.X, rightPoint.Y).B == 0)
					{
						points.Add(rightPoint);
					}
				}
				
				var downPoint = new Point(point.X,point.Y+1);
				if (!points.Contains(downPoint))
				{
					if (downPoint.Y < bitmap.Height && bitmap.GetPixel(downPoint.X, downPoint.Y).B == 0)
					{
						points.Add(downPoint);
					}
				}
				
				var leftPoint = new Point(point.X-1,point.Y);
				if (!points.Contains(leftPoint))
				{
					if (leftPoint.X >= 0 && bitmap.GetPixel(leftPoint.X, leftPoint.Y).B == 0)
					{
						points.Add(leftPoint);
					}
				}
			}

			return points;
		}
		
		/// <summary>
		/// Метод, который обрезает изображение
		/// </summary>
		private Bitmap CutBitmap(Bitmap bitmap)
		{
			var miny = 0;
			var maxy = 0;
			for (int y = 0; y < bitmap.Height; y++)
			{
				if (IsContainsBlackPixelOnLineX(bitmap, y))
				{
					miny = y;
					break;
				}
			}

			for (int y = bitmap.Height-1; y >= 0; y--)
			{
				if (IsContainsBlackPixelOnLineX(bitmap,y))
				{
					maxy = y;
					break;
				}
			}
			
			var minx = 0;
			for (int x = 0; x < bitmap.Width; x++)
			{
				if (IsContainsBlackPixelOnLineY(bitmap, x))
				{
					minx = x;
					break;
				}
			}
			
			var maxx = 0;
			for (int x = bitmap.Width-1; x >= 0; x--)
			{
				if (IsContainsBlackPixelOnLineY(bitmap,x))
				{
					maxx = x;
					break;
				}
			}

			if (maxx - minx < 3 || maxy - miny < 3)
			{
				return null;
			}
			return CropImage(bitmap, new Rectangle(minx, miny, maxx - minx, maxy - miny));
		}
		
		/// <summary>
		/// Метод получения признака содержания черного пикселя на линии X
		/// </summary>
		/// <param name="bitmap">Исходное изображение</param>
		/// <param name="y">Точка Y</param>
		/// <returns>true - если на линии Y есть точка с черным пикселем</returns>
		private bool IsContainsBlackPixelOnLineX(Bitmap bitmap,int y)
		{
			for (int x = 0; x < bitmap.Width; x++)
			{
				if (bitmap.GetPixel(x, y).B == 0)
				{
					return true;
				}
			}

			return false;
		}
		
		/// <summary>
		/// Метод получения признака содержания черного пикселя на линии Y
		/// </summary>
		/// <param name="bitmap">Исходное изображение</param>
		/// <param name="x">Точка X</param>
		/// <returns>true - если на линии X есть точка с черным пикселем</returns>
		private bool IsContainsBlackPixelOnLineY(Bitmap bitmap,int x)
		{
			for (int y = 0; y < bitmap.Height; y++)
			{
				if (bitmap.GetPixel(x, y).B == 0)
				{
					return true;
				}
			}

			return false;
		}
		
		/// <summary>
		/// Метод обрезания изображения
		/// </summary>
		/// <param name="source">Исходное изображение</param>
		/// <param name="section">Область которую необходимо оставить</param>
		private Bitmap CropImage(Bitmap source, Rectangle section)
		{
			Bitmap bmp = new Bitmap(section.Width-1, section.Height-1);
			Graphics g = Graphics.FromImage(bmp);
			g.DrawImage(source, -1, -1, section, GraphicsUnit.Pixel);
			return bmp;
		}
	}
}