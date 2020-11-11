using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using gk_p6;

namespace gk_p6
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			List<Point> points = new List<Point>();
			Random rng = new Random();
			//for(int i = 0; i < 25; i++)
			//         {
			//	//points.Add(new Point(rng.Next(450), rng.Next(800)));
			//	points.Add(new Point(i, i));
			//         }
			points.Add(new Point(100, 100));
            points.Add(new Point(300, 100));
            points.Add(new Point(300, 300));
            //points.Add(new Point(200, 150));
            DrawPoints(points);
		}
		private void DrawPoints(List<Point> points)
		{
            Path path = MakeCurve(Bezier.GetPoints(points).ToArray(), 0.01);
            path.Stroke = System.Windows.Media.Brushes.Black;
            path.StrokeThickness = 2;
            Canvas.SetLeft(path, points.ElementAt(0).X);
            Canvas.SetTop(path, points.ElementAt(0).Y);
            Canvas.Children.Add(path);
            List<Point> bezier = Bezier.DrawBezier(points);
            foreach (var item in bezier)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = System.Windows.Media.Brushes.Black;
                ellipse.StrokeThickness = 2;
                ellipse.Stroke = System.Windows.Media.Brushes.Black;
                ellipse.Width = 10;
                ellipse.Height = 10;
                Canvas.SetLeft(ellipse, item.X);
                Canvas.SetTop(ellipse, item.Y);
                Canvas.Children.Add(ellipse);
            }
            foreach (var point in points)
            {
                Ellipse ellipse = new Ellipse();
                SolidColorBrush color = new SolidColorBrush();
                color.Color = Color.FromArgb(255, 255, 255, 0);
                ellipse.Fill = color;
                ellipse.StrokeThickness = 2;
                ellipse.Stroke = System.Windows.Media.Brushes.Red;
                ellipse.Width = 16;
                ellipse.Height = 16;
                Canvas.SetLeft(ellipse, point.X);
                Canvas.SetTop(ellipse, point.Y);
                Canvas.Children.Add(ellipse);
            }
        }
        private Path MakeBezierPath(Point[] points)
		{
			Path path = new Path();
			PathGeometry pathGeometry = new PathGeometry();
			path.Data = pathGeometry;
			PathFigure pathFigure = new PathFigure();
			pathGeometry.Figures.Add(pathFigure);
			pathFigure.StartPoint = points[0];
			PathSegmentCollection pathSegments = new PathSegmentCollection();
			pathFigure.Segments = pathSegments;
			PointCollection pointsCollection = new PointCollection(points.Length - 1);
			for(int i = 1; i < points.Length; i++)
            {
				pointsCollection.Add(points[i]);
            }
			PolyBezierSegment polyBezierSegment = new PolyBezierSegment();
			polyBezierSegment.Points = pointsCollection;
			pathSegments.Add(polyBezierSegment);
			return path;
        }
		private Point[] MakeCurvePoints(Point[] points, double tension)
		{
			if (points.Length < 2) return null;
			double control_scale = tension / 0.5 * 0.175;

			// Make a list containing the points and
			// appropriate control points.
			List<Point> result_points = new List<Point>();
			result_points.Add(points[0]);

			for (int i = 0; i < points.Length - 1; i++)
			{
				// Get the point and its neighbors.
				Point pt_before = points[Math.Max(i - 1, 0)];
				Point pt = points[i];
				Point pt_after = points[i + 1];
				Point pt_after2 = points[Math.Min(i + 2, points.Length - 1)];

				double dx1 = pt_after.X - pt_before.X;
				double dy1 = pt_after.Y - pt_before.Y;

				Point p1 = points[i];
				Point p4 = pt_after;

				double dx = pt_after.X - pt_before.X;
				double dy = pt_after.Y - pt_before.Y;
				Point p2 = new Point(
					pt.X + control_scale * dx,
					pt.Y + control_scale * dy);

				dx = pt_after2.X - pt.X;
				dy = pt_after2.Y - pt.Y;
				Point p3 = new Point(
					pt_after.X - control_scale * dx,
					pt_after.Y - control_scale * dy);

				// Save points p2, p3, and p4.
				result_points.Add(p2);
				result_points.Add(p3);
				result_points.Add(p4);
			}

			// Return the points.
			return result_points.ToArray();
		}
		private Path MakeCurve(Point[] points, double tension)
		{
			if (points.Length < 2) return null;
			Point[] result_points = MakeCurvePoints(points, tension);

			// Use the points to create the path.
			return MakeBezierPath(result_points.ToArray());
		}
	}
}
