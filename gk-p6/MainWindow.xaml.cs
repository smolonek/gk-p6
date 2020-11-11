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
using System.Diagnostics;
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
        List<Point> controlPoints = new List<Point>();
        List<Ellipse> ellipses = new List<Ellipse>();
        public MainWindow()
        {
            InitializeComponent();
            //List<Point> points = new List<Point>();
            //points.Add(new Point(100, 100));
            //points.Add(new Point(300, 100));
            //points.Add(new Point(300, 300));
            //points.Add(new Point(200, 150));
            //DrawPoints(points);
        }
        private void DrawPoints(List<Point> points)
        {
            Path path = MakeCurve(Bezier.GetPoints(points).ToArray(), 0.01);
            path.Stroke = System.Windows.Media.Brushes.Black;
            path.StrokeThickness = 2;
            Canvas.Children.Add(path);
            //List<Point> bezier = Bezier.GetPoints(points);
            //foreach (var item in bezier)
            //{
            //    Ellipse ellipse = new Ellipse();
            //    ellipse.Fill = System.Windows.Media.Brushes.Black;
            //    ellipse.StrokeThickness = 2;
            //    ellipse.Stroke = System.Windows.Media.Brushes.Black;
            //    ellipse.Width = 10;
            //    ellipse.Height = 10;
            //    Canvas.SetLeft(ellipse, item.X);
            //    Canvas.SetTop(ellipse, item.Y);
            //    Canvas.Children.Add(ellipse);
            //}
            //ClearControlPoints();
            foreach (var point in points)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = System.Windows.Media.Brushes.LightGray;
                ellipse.StrokeThickness = 2;
                ellipse.Stroke = System.Windows.Media.Brushes.Gray;
                ellipse.Width = 16;
                ellipse.Height = 16;
                Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
                Canvas.SetTop(ellipse, point.Y - ellipse.Width / 2);
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
            for (int i = 1; i < points.Length; i++)
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

        private void ClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
            controlPoints.Clear();
            ellipses.Clear();
        }


        //      private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        //      {
        //	Ellipse point = new Ellipse();
        //	point.Fill = System.Windows.Media.Brushes.LightGray;
        //	point.StrokeThickness = 2;
        //	point.Stroke = System.Windows.Media.Brushes.Gray;
        //	point.Width = 16;
        //	point.Height = 16;
        //	Canvas.SetLeft(point, e.GetPosition(this).X - point.Width / 2);
        //	Canvas.SetTop(point, e.GetPosition(this).Y - point.Width / 2);
        //	Canvas.Children.Add(point);
        //}

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse point = new Ellipse();
            point.Fill = System.Windows.Media.Brushes.LightGray;
            point.StrokeThickness = 2;
            point.Stroke = System.Windows.Media.Brushes.Gray;
            point.Width = 16;
            point.Height = 16;
            point.PreviewMouseMove += ControlPoint_MouseMove;
            double x = e.GetPosition(this).X, y = e.GetPosition(this).Y;
            Canvas.SetLeft(point, x - point.Width / 2);
            Canvas.SetTop(point, y - point.Width / 2);
            Canvas.Children.Add(point);
            Canvas.SetZIndex(point, 9999);
            controlPoints.Add(new Point(x, y));
            ellipses.Add(point);
        }

        private void DrawBezier_Click(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
            DrawPoints(controlPoints);
            for (int i = Canvas.Children.Count - 1; i >= 0; i--)
            {
                if (Canvas.Children[i].GetType() == typeof(Ellipse))
                    Canvas.Children.Remove(Canvas.Children[i]);
            }
            ellipses.Clear();
            foreach (var item in controlPoints)
            {
                Ellipse point = new Ellipse();
                point.Fill = System.Windows.Media.Brushes.LightGray;
                point.StrokeThickness = 2;
                point.Stroke = System.Windows.Media.Brushes.Gray;
                point.Width = 16;
                point.Height = 16;
                point.MouseMove += ControlPoint_MouseMove;
                double x = item.X, y = item.Y;
                Canvas.SetLeft(point, x - point.Width / 2);
                Canvas.SetTop(point, y - point.Width / 2);
                Canvas.Children.Add(point);
                ellipses.Add(point);
            }
        }
        private void ControlPoint_MouseMove(object sender, MouseEventArgs e)
        {
            var ellipse = (Ellipse)sender;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                CordsLabel.Content = string.Format("X: {0}, Y: {1}", e.GetPosition(this).X, e.GetPosition(this).Y);
                //MessageBox.Show("PO");
                int index = 0;
                index = ellipses.IndexOf(ellipse);
                Canvas.SetLeft(ellipse, e.GetPosition(this).X - ellipse.ActualWidth / 2);
                Canvas.SetTop(ellipse, e.GetPosition(this).Y - ellipse.ActualHeight / 2);
                Point point = new Point();
                point.X = e.GetPosition(this).X;
                point.Y = e.GetPosition(this).Y;
                controlPoints[index] = point;
                DeletePath();
                //ClearControlPoints();
                //Canvas.Children.Clear();
                DrawPoints(controlPoints);
            }
        }
        private void DeletePath()
        {
            for (int i = Canvas.Children.Count - 1; i >= 0; i--)
            {
                if(Canvas.Children[i].GetType() == typeof(Path))
                    Canvas.Children.Remove(Canvas.Children[i]);
            }
        }
        private void ClearControlPoints()
        {
            List<Ellipse> tmp = new List<Ellipse>();
            for (int i = Canvas.Children.Count - 1; i >= 0; i--)
            {
                Trace.WriteLine(Canvas.Children[i].GetType().ToString() + " : " + Canvas.Children.Count);
                if (Canvas.Children[i].GetType() == typeof(Ellipse))
                    tmp.Add((Ellipse)Canvas.Children[i]);
            }
            foreach(var item in tmp)
            {
                Canvas.Children.Remove(item);
            }
        }
        private void ClearControlPoints_Click(object sender, RoutedEventArgs e)
        {
            ClearControlPoints();
        }
    }
}
