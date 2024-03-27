using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Imaging;
using Tesseract;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Data;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private System.Drawing.Point _currentPoint;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            _currentPoint = new System.Drawing.Point();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Tick += CheckScreen;
            _timer.Start();

            this.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                paintCanvas.Children.Clear();
                _timer.Start();
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                _currentPoint = new System.Drawing.Point((int)e.GetPosition(this).X, (int)e.GetPosition(this).Y);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line
                {
                    Stroke = Brushes.Black,
                    X1 = _currentPoint.X,
                    Y1 = _currentPoint.Y,
                    X2 = e.GetPosition(this).X,
                    Y2 = e.GetPosition(this).Y,
                    StrokeThickness = 8
                };

                _currentPoint = new System.Drawing.Point((int)e.GetPosition(this).X, (int)e.GetPosition(this).Y);

                paintCanvas.Children.Add(line);
            }
        }

        private void CheckScreen(object sender, EventArgs e)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)this.Width, (int)this.Height, 90, 90, PixelFormats.Pbgra32);
            rtb.Render(this);

            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));

            using (Stream stm = File.Create("img.png"))
            {
                png.Save(stm);
            }

            using (var engine = new TesseractEngine(@"C:\Учеба\С#\Calculator\Calculator\Tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile("img.png"))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        var onlyDigitsAndOperators = Regex.Replace(text, @"[^0-9+-=]", "");
                        if (onlyDigitsAndOperators.Contains("="))
                        {
                            var expression = onlyDigitsAndOperators.Split('=')[0];
                            var result = new DataTable().Compute(expression, null);
                            MessageBox.Show(result.ToString(), "Tesseract Output");
                            _timer.Stop();
                        }
                    }
                }
            }
        }
    }
}