using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows;
using Point = System.Windows.Point;

namespace gk_p6
{
	public class Bezier
	{
		/// <summary>
		/// O ile zwiększa się t ze wzoru
		/// </summary>
		private const float dt = 0.01f;
		private static long BinomialCoefficient(long n, long k)
		{
			//if (k > n - k) k = n - k;
			//long outcome = 1;
			//if (n == k || k == 0)
			//	return 1;
			//for(int i = 0; i <= k; ++i)
			//{
			//	outcome *= n - (k - i);
			//	outcome /= i;
			//}
			//return outcome;
			if ((n == k) || (k == 0))
				return 1;
			else
				return BinomialCoefficient(n - 1, k) + BinomialCoefficient(n - 1, k - 1);
		}
		private static double GetX(float t, Point[] points)
		{
			double sum = 0;
			long n = points.Length - 1;
			for(long i = 0; i < n; i++)
			{
				sum += points[i].X * BinomialCoefficient(n, i) * Math.Round(Math.Pow(t, i), 2) * Math.Round(Math.Pow((1 - t), (n - i)), 2);
			}
			return Math.Round(sum, 2);
		}
		private static double GetY(float t, Point[] points)
		{
			double sum = 0;
			long n = points.Length - 1;
			Trace.WriteLine("-----------START-----------");
			for (long i = 0; i < n; i++)
			{
				sum += points[i].Y * BinomialCoefficient(n, i) * Math.Round(Math.Pow(t, i), 2) * Math.Round(Math.Pow((1 - t), (n - i)), 2);
				Trace.WriteLine(string.Format("{0} * {1} * {2} * {3}", points[i].Y, BinomialCoefficient(n, i), Math.Round(Math.Pow(t, i), 2), Math.Round(Math.Pow((1 - t), (n - i)), 2)));
			}
			Trace.WriteLine("-----------END-----------");
			return Math.Round(sum, 2);
		}
		private static double X(double t, List<Point> points)
		{
			//todo: sprobowac foreachami, elementat(item) albo w ogole casteljau czy jak mu tam
			double sum = 0.0;
			int n = points.Count - 1;
			foreach (var item in points)
			{
				int i = points.IndexOf(item);
				sum += item.X * BinomialCoefficient(n, i) * Math.Pow(t, i) * Math.Pow(1 - t, n - i);
			}
			return sum;
		}
		private static double Y(double t, List<Point> points)
		{
			double sum = 0.0;
			int n = points.Count - 1;
			foreach(var item in points)
            {
				int i = points.IndexOf(item);
				sum += item.Y * BinomialCoefficient(n, i) * Math.Pow(t, i) * Math.Pow(1 - t, n - i);
            }
			return sum;
		}
		public static List<Point> GetPoints(List<Point> controlPoints)
		{
			List<Point> points = new List<Point>();
			Point[] ctrl = controlPoints.ToArray();
			for (float t = 0.0f; t < 1.0; t += dt)
			{
				points.Add(new Point(GetX(t, ctrl), GetY(t, ctrl)));
			}
			points.Add(new Point(GetX(1.0f, ctrl), GetY(1.0f, ctrl)));
			return points;
		}
		public static List<Point> DrawBezier(List<Point> controlPoints)
		{
			// Draw the curve.
			List<Point> points = new List<Point>();
			for (float t = 0.0f; t < 1.0; t += dt)
			{
				points.Add(new Point(
					X(t, controlPoints),
					Y(t, controlPoints)));
			}

			// Connect to the final point.
			points.Add(new Point(
				X(1.0f, controlPoints),
				Y(1.0f, controlPoints)));
			return points;
		}
	}
}
