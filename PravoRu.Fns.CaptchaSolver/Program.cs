using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;

namespace PravoRu.Fns.CaptchaSolver
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ResizeDataSet();
			
			if (Directory.Exists("Samples"))
			{
				Directory.Delete("Samples", true);
			}

			Directory.CreateDirectory("Samples");
			foreach (var file in Directory.GetFiles(@"..\..\Samples"))
			{
				File.Copy(file, $"Samples\\{new FileInfo(file).Name}");
			}
			
			var i = 0;
			foreach (var path in Directory.GetFiles("Samples"))
			{
				var pathDirectory = $"{new FileInfo(path).Name}";
				if (Directory.Exists(pathDirectory))
				{
					Directory.Delete(pathDirectory, true);
				}
				Directory.CreateDirectory(pathDirectory);
				Test(path, pathDirectory);
			}
		}

		private static void DownloadCaptchas()
		{
			var seconds = (DateTime.UtcNow - DateTime.MinValue).TotalSeconds;
			var token = new WebClient().DownloadString(
				$"https://service.nalog.ru/static/captcha.bin?{seconds}");
			var bytes = new WebClient().DownloadData($"https://service.nalog.ru/static/captcha.bin?r={seconds}&a={token}&version=3");
			var image = Bitmap.FromStream(new MemoryStream(bytes));
			image.Save($"..\\..\\Samples\\{seconds}.bmp");
		}

		private static void Test(string path, string pathDirectory)
		{
			Bitmap b = new Bitmap(path);

			b.Save(pathDirectory+@"\original.bmp");
			
			SetBlackWhiteBitmap(b);
			
			SaveBySquare(b, pathDirectory);
		}
		
		public static void Run(Bitmap b, string pathDirectory)
		{
			SetBlackWhiteBitmap(b);
			SaveBySquare(b, pathDirectory);
		}
		
		private static void ResizeDataSet()
		{
			foreach (var file in Directory.GetFiles(@"C:\Users\1\Desktop\DataSet\full"))
			{
				Bitmap b = new Bitmap(file);
				
				var resizedBitmap = Resize(b);
				
				SetBlackWhiteBitmap(resizedBitmap);
				b.Dispose();
				File.Delete(file);
				resizedBitmap.Save(file);
			}
		}

		private static void SetBlackWhiteBitmap(Bitmap b)
		{
			for (int x = 0; x < b.Width; x++)
			{
				for (int y = 0; y < b.Height; y++)
				{
					var pixel = b.GetPixel(x, y);
					if (pixel.R > 127 && pixel.G > 127 && pixel.B > 127)
					{
						b.SetPixel(x, y, Color.White);
					}
					else
					{
						b.SetPixel(x, y, Color.Black);
					}
				}
			}
		}
		
		public static void SaveBySquare(Bitmap cutedBitmap, string pathDirectory)
		{
			var bitmaps = GetBitmapsBySquare(cutedBitmap);
			for (int i = 0; i < 3; i++)
			{
				var resultBitmaps = new List<Bitmap>(6);
				if (bitmaps.Count < 6)
				{
					foreach (var bitmap in bitmaps)
					{
						if (bitmap.Width > 35)
						{
							var resized = GetBitmapsBySquare(bitmap);
							foreach (var bitmap1 in resized)
							{
								resultBitmaps.Add(bitmap1);
							}
						}
						else
						{
							resultBitmaps.Add(bitmap);
						}
					}

					bitmaps = resultBitmaps;
				}
			}
			var j = 0;
			foreach (var bitmap in bitmaps)
			{
				Resize(bitmap).Save(pathDirectory+$@"\{++j}.bmp");
			}
			
		}
		
		private static Bitmap Resize (Bitmap original)
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

		private static List<Bitmap> GetBitmapsBySquare(Bitmap cutedBitmap)
		{
			var bitmaps = new List<Bitmap>(10);
			var usedPointArray = new List<Point>();
			for (int x = 1; x < cutedBitmap.Width; x++)
			{
				for (int y = 1; y < cutedBitmap.Height; y++)
				{
					if (usedPointArray.Contains(new Point(x, y)))
					{
						continue;
					}

					if (cutedBitmap.GetPixel(x, y).B == 0)
					{
						List<Point> pointArray = GetShape(cutedBitmap, x, y);
						var maxx = pointArray.Max(a => a.X);
						usedPointArray.AddRange(pointArray);
						if (pointArray.Count < 120)
						{
							continue;
						}

						Bitmap digit = new Bitmap(cutedBitmap.Width, cutedBitmap.Height);

						for (int x1 = 0; x1 < digit.Width; x1++)
						{
							for (int y1 = 0; y1 < digit.Height; y1++)
							{
								digit.SetPixel(x1,y1, Color.White);
							}
						}
						
						foreach (var point in pointArray)
						{
							digit.SetPixel(point.X, point.Y, Color.Black);
						}
						
						//Удалим линии по ширине и высоте
						CutLittleLinesX(digit);
						CutLittleLinesY(digit);

						var cutedBitmap2 = CutBitmap(digit);
						if (cutedBitmap2 != null)
						{
							bitmaps.Add(cutedBitmap2);
						}
					}
				}
			}

			return bitmaps;
		}

		private static void CutLittleLinesX(Bitmap digit)
		{
			//Попробуем убрать тонкие линии меньше 2х пикселей
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

						if (localPixels.Count > 4)
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
		
		private static void CutLittleLinesY(Bitmap digit)
		{
			//Попробуем убрать тонкие линии меньше 2х пикселей
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

						if (localPixels.Count > 4)
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

		private static void SaveByWidth(Bitmap cutedBitmap, string pathDirectory)
		{
			pathDirectory += @"\bywidth";
			var usedPointArray = new List<Point>();
			var j = 0;

			for (int x = 1; x < cutedBitmap.Width; x++)
			{
				for (int y = 1; y < cutedBitmap.Height; y++)
				{
					if (usedPointArray.Contains(new Point(x, y)))
					{
						continue;
					}

					if (cutedBitmap.GetPixel(x, y).B == 0)
					{
						List<Point> pointArray = GetShape(cutedBitmap, x, y);
						var maxx = pointArray.Max(a => a.X);
						usedPointArray.AddRange(pointArray);
						if (pointArray.Count < 2)
						{
							continue;
						}

						var width = maxx - x;
						
						if ( width < 6)
						{
							continue;
						}
						
						var bb1 = CropImage(cutedBitmap, new Rectangle(x , 0, width, cutedBitmap.Height));
						bb1.Save(pathDirectory + $@"\source_{j}_cutted.bmp");

						if (width > 33 && width < 60)
						{
							for (int i = 0; i < 2; i++)
							{
								var bb = CropImage(cutedBitmap, new Rectangle(x + (width/2)*i, 0, width/2, cutedBitmap.Height));
								bb.Save(pathDirectory + $@"\{++j}_cutted.bmp");
							}
						}

						if (width > 60)
						{
							for (int i = 0; i < 3; i++)
							{
								var bb = CropImage(cutedBitmap, new Rectangle(x + (width/3)*i, 0, width/3, cutedBitmap.Height));
								bb.Save(pathDirectory + $@"\{++j}_cutted.bmp");
							}
						}
					}
				}
			}
		}

		private static List<Point> GetShape(Bitmap b, int x, int y)
		{
			List<Point> points =new List<Point>(300);
			points.Add(new Point(x,y));
			for (int i = 0; i < points.Count; i++)
			{
				var point = points[i];
				var upPoint = new Point(point.X,point.Y-1);
				if (!points.Contains(upPoint))
				{
					if (upPoint.Y >= 0 && b.GetPixel(upPoint.X, upPoint.Y).B == 0)
					{
						points.Add(upPoint);
					}
				}
				
				
				var rightPoint = new Point(point.X+1,point.Y);
				if (!points.Contains(rightPoint))
				{
					if (rightPoint.X < b.Width && b.GetPixel(rightPoint.X, rightPoint.Y).B == 0)
					{
						points.Add(rightPoint);
					}
				}
				
				var downPoint = new Point(point.X,point.Y+1);
				if (!points.Contains(downPoint))
				{
					if (downPoint.Y < b.Height && b.GetPixel(downPoint.X, downPoint.Y).B == 0)
					{
						points.Add(downPoint);
					}
				}
				
				var leftPoint = new Point(point.X-1,point.Y);
				if (!points.Contains(leftPoint))
				{
					if (leftPoint.X >= 0 && b.GetPixel(leftPoint.X, leftPoint.Y).B == 0)
					{
						points.Add(leftPoint);
					}
				}
			}

			return points;
		}

		private static Bitmap CutBitmap(Bitmap b)
		{
			var miny = 0;
			var maxy = 0;
			for (int y = 0; y < b.Height; y++)
			{
				if (IsContainsBlackPixelOnLineX(b, y))
				{
					miny = y;
					break;
				}
			}

			for (int y = b.Height-1; y >= 0; y--)
			{
				if (IsContainsBlackPixelOnLineX(b,y))
				{
					maxy = y;
					break;
				}
			}
			
			var minx = 0;
			for (int x = 0; x < b.Width; x++)
			{
				if (IsContainsBlackPixelOnLineY(b, x))
				{
					minx = x;
					break;
				}
			}
			
			var maxx = 0;
			for (int x = b.Width-1; x >= 0; x--)
			{
				if (IsContainsBlackPixelOnLineY(b,x))
				{
					maxx = x;
					break;
				}
			}

			if (maxx - minx < 3 || maxy - miny < 3)
			{
				return null;
			}
			return CropImage(b, new Rectangle(minx, miny, maxx - minx, maxy - miny));
		}

		private static bool IsContainsBlackPixelOnLineX(Bitmap b,int y)
		{
			for (int i = 0; i < b.Width; i++)
			{
				if (b.GetPixel(i, y).B == 0)
				{
					return true;
				}
			}

			return false;
		}
		
		private static bool IsContainsBlackPixelOnLineY(Bitmap b,int x)
		{
			for (int i = 0; i < b.Height; i++)
			{
				if (b.GetPixel(x, i).B == 0)
				{
					return true;
				}
			}

			return false;
		}

		public static Bitmap CropImage(Bitmap source, Rectangle section)
		{
			// An empty bitmap which will hold the cropped image
			Bitmap bmp = new Bitmap(section.Width-1, section.Height-1);

			Graphics g = Graphics.FromImage(bmp);

			// Draw the given area (section) of the source image
			// at location 0,0 on the empty bitmap (bmp)
			g.DrawImage(source, -1, -1, section, GraphicsUnit.Pixel);

			return bmp;
		}

		private static void MergeHorizontal(Bitmap b)
		{
			for (int y = 0; y < b.Height; y++)
			{
				for (int x = 0; x < b.Width - 3; x++)
				{
					var pixel1 = b.GetPixel(x, y);
					if (pixel1.R == 255)
					{
						continue;
					}
					var pixel2 = b.GetPixel(x + 1, y);
					if (!pixel1.Equals(pixel2))
					{
						if (b.GetPixel(x + 2, y).Equals(pixel1))
						{
							b.SetPixel(x + 1, y, pixel1);
							b.SetPixel(x + 2, y, pixel1);
						}
					}
				}
			}
		}

		private static  void MergeVertical(Bitmap b)
		{
			for (int x = 0; x < b.Width; x++)
			{
				for (int y = 0; y < b.Height - 3; y++)
				{
					var pixel1 = b.GetPixel(x, y);
					if (pixel1.R == 255)
					{
						continue;
					}
					var pixel2 = b.GetPixel(x , y+1);
					if (!pixel1.Equals(pixel2))
					{
						if (b.GetPixel(x , y+2).Equals(pixel1))
						{
							b.SetPixel(x,y + 1, pixel1);
							b.SetPixel(x,y + 2, pixel1);
						}
					}
				}
			}
		}
	}
}