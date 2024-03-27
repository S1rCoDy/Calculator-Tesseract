using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using tessnet2;

namespace kalkulatur
{
    public partial class MainWindow : Window
    {
        private System.Windows.Point previousPoint;
        private DrawingAttributes drawingAttributes;
        private Tesseract ocr;

        public MainWindow()
        {
            InitializeComponent();

            drawingAttributes = new DrawingAttributes
            {
                Color = System.Windows.Media.Colors.Black,
                Height = 2,
                Width = 2
            };

            // Инициализация объекта Tessnet2
            ocr = new Tesseract();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point currentPoint = e.GetPosition(canvas);

                Line line = new Line
                {
                    X1 = previousPoint.X,
                    Y1 = previousPoint.Y,
                    X2 = currentPoint.X,
                    Y2 = currentPoint.Y,
                    Stroke = new SolidColorBrush(drawingAttributes.Color),
                    StrokeThickness = drawingAttributes.Width
                };

                previousPoint = currentPoint;
                canvas.Children.Add(line);
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            previousPoint = e.GetPosition(canvas);
        }

        private void RecognizeButton_Click(object sender, RoutedEventArgs e)
        {
            // Код для распознавания с помощью Tessnet2
            MessageBox.Show("Распознавание с помощью Tessnet2");
        }
    }
}
