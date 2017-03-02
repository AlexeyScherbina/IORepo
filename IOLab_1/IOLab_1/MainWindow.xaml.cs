using System;
using System.Collections.Generic;
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

namespace IOLab_1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double moveX = 0;
        double moveY = 0;
        double width = 0;
        double height = 0;
        double YMax = 1;
        double YMin = -1;
        double xMin = -1;
        double xMax = 1;
        double xScale = 0;
        double yScale = 0;
        double x0 = 0;
        double y0 = 0;
        List<double> realRoots;

        public MainWindow()
        {
            InitializeComponent();
        }
        void GraphicParams(double x)
        {
            width = canvas.ActualWidth;
            height = canvas.ActualHeight;
            try
            {
                YMax = double.Parse(textBoxYmax.Text) / slider.Value + moveY;
            }
            catch (Exception) { }
            try
            {
                YMin = double.Parse(textBoxYmin.Text) / slider.Value + moveY;
            }
            catch (Exception) { }
            xMin = -1 / slider.Value + moveX;
            xMax = x / slider.Value + moveX;
            xScale = width / (xMax - xMin);
            yScale = height / (YMax - YMin);
            x0 = -xMin * xScale;
            y0 = YMax * yScale;
        }
        void AddLine(Brush stroke, double x1, double y1, double x2, double y2)
        {
            canvas.Children.Add(new Line() { X1 = x1, X2 = x2, Y1 = y1, Y2 = y2, Stroke = stroke });
        }

        void AddText(string text, double x, double y)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = Brushes.Black;
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            canvas.Children.Add(textBlock);
        }

        void DrawGraph()
        {
            canvas.Children.Clear();
            int margin = 0;
            double a = 1;
            if (test)
            {
                a = 1.5;
            }
            else
            {
                a = 1;
            }
            GraphicParams(a);

            canvas.Children.Add(new Rectangle()
            {
                Width = width,
                Height = height,
                Margin = new Thickness(0, 0, 0, 0),
                Fill = Brushes.Wheat,
                Opacity = 0.01
            });
            double xStep = 1;
            while (xStep * xScale < 25)
                xStep *= 10;
            while (xStep * xScale > 250)
                xStep /= 10;
            for (double dx = xStep; dx <= xMax; dx += xStep)
            {
                double x = x0 + dx * xScale;
                AddLine(Brushes.LightGray, x + margin, 0, x + margin, height);
                AddText(string.Format("{0:0.###}", dx), x + 1 + margin, y0 - 2);
            }
            for (double dx = -xStep; dx >= xMin; dx -= xStep)
            {
                double x = x0 + dx * xScale;
                AddLine(Brushes.LightGray, x + margin, 0, x + margin, height);
                AddText(string.Format("{0:0.###}", dx), x + 1 + margin, y0 - 2);
            }
            double yStep = 1;
            while (yStep * yScale < 20)
                yStep *= 10;
            while (yStep * yScale > 200)
                yStep /= 10;
            for (double dy = yStep; dy <= YMax; dy += yStep)
            {
                double y = y0 - dy * yScale;
                AddLine(Brushes.LightGray, 0 + margin, y, width + margin, y);
                if (y < height - 12)
                {
                    AddText(string.Format("{0:0.###}", dy), x0 + 2 + margin, y - 2);
                }
            }
            for (double dy = -yStep; dy >= YMin; dy -= yStep)
            {
                double y = y0 - dy * yScale;
                AddLine(Brushes.LightGray, 0 + margin, y, width + margin, y);
                if (y < height - 12)
                {
                    AddText(string.Format("{0:0.###}", dy), x0 + 2 + margin, y - 2);
                }
            }
            if (xMin * xMax < 0)
            {
                AddLine(Brushes.Black, x0 + margin, 0, x0 + margin, height);
                AddText("Y", x0 - 10 + margin, 2);
            }
            else
            {
                AddLine(Brushes.Black, margin, 0, margin, height);
                AddText("Y", 4, 2);
                for (double dy = yStep; dy < YMax; dy += yStep)
                {
                    double y = y0 - dy * yScale;
                    if (y < height - 12)
                    {
                        AddText(string.Format("{0:0.###}", dy), margin + 2, y - 2);
                    }
                }
                for (double dy = -yStep; dy > YMin; dy -= yStep)
                {
                    double y = y0 - dy * yScale;
                    AddText(string.Format("{0:0.###}", dy), margin + 2, y - 2);
                }
            }
            if (YMin * YMax < 0)
            {
                AddLine(Brushes.Black, 0 + margin, y0, width + margin, y0);
                AddText("X", width - 10 + margin, y0 - 14);
            }
            else
            {
                AddLine(Brushes.Black, 0 + margin, height, width + margin, height);
                AddText("X", width - 10 + margin, height - 14);
                for (double dx = xStep; dx <= xMax; dx += xStep)
                {
                    double x = x0 + dx * xScale;
                    if (x > -1)
                    {
                        AddText(string.Format("{0:0.###}", dx), x + 1 + margin, height - 20);
                    }
                }
                for (double dx = -xStep; dx >= xMin; dx -= xStep)
                {
                    double x = x0 + dx * xScale;
                    if (x > -1)
                    {
                        AddText(string.Format("{0:0.###}", dx), x + 1 + margin, height - 20);
                    }
                }
            }
            AddText("0", x0 + 1 + margin, y0 - 2);

        }

        void DrawRoots()
        {
            foreach (double x in realRoots)
            {
                canvas.Children.Add(new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    Margin = new Thickness(x0 + x * xScale - 3, y0 - 3, 0, 0),
                    Fill = Brushes.Black
                });
            }
        }
        void DrawGraphic(List<Solution> l, Color c, Function f)
        {
            int margin = 0;
            double a = 1;
            if (test)
            {
                a = 1.5;
            }
            else
            {
                a = 1;
            }
            GraphicParams(a);
            double e = double.Parse(textBoxE.Text);
            if (e < 0.0001) e = 0.0001;
            double x = -1 + e;
            double y = f(x, int.Parse(comboBoxK.Text));
            for (double i = -1 + e; i < a; i += e)
            {
                try
                {
                    canvas.Children.Add(new Line()
                    {
                        X1 = x0 + margin + x * xScale,
                        X2 = x0 + margin + i * xScale,
                        Y1 = y0 - y * yScale,
                        Y2 = y0 - f(i, int.Parse(comboBoxK.Text)) * yScale,
                        StrokeThickness = 2,
                        Stroke = new SolidColorBrush(c)
                    });
                    x = i;
                    y = f(i, int.Parse(comboBoxK.Text));

                }
                catch (Exception) { }
            }
            DrawRoots();
        }
    }
}
