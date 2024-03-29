using Microsoft.Win32;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows;
using Tesseract;
using System.Windows.Controls;


namespace Calculator
{
    public partial class MainWindow : Window
    {
        private System.Drawing.Point _currentPoint;
        private DispatcherTimer _timer;
        private bool _isTabPressed;

        public MainWindow()
        {
            InitializeComponent();
            _currentPoint = new System.Drawing.Point();
            this.KeyDown += MainWindow_KeyDown;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    ImageBrush myBrush = new ImageBrush();
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    myBrush.ImageSource = image.Source;
                    paintCanvas.Background = myBrush;
                }
            }
            else if (e.Key == Key.Tab)
            {
                paintCanvas.Children.Clear();
                paintCanvas.Background = Brushes.White;
                _isTabPressed = true;
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                _currentPoint = new System.Drawing.Point((int)e.GetPosition(paintCanvas).X, (int)e.GetPosition(paintCanvas).Y);
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
                    X2 = e.GetPosition(paintCanvas).X,
                    Y2 = e.GetPosition(paintCanvas).Y,
                    StrokeThickness = 8
                };

                _currentPoint = new System.Drawing.Point((int)e.GetPosition(paintCanvas).X, (int)e.GetPosition(paintCanvas).Y);

                paintCanvas.Children.Add(line);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_isTabPressed)
            {
                _isTabPressed = false;
                return;
            }
            if(paintCanvas.ActualWidth <= 0 && paintCanvas.ActualHeight <= 0)
            {
                return;
            }
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)paintCanvas.ActualWidth, (int)paintCanvas.ActualHeight, 64, 64, PixelFormats.Pbgra32);
            rtb.Render(paintCanvas);

            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));

            using (Stream stm = File.Create("img.png"))
            {
                png.Save(stm);
            }

            using (var engine = new TesseractEngine(@"C:\Учеба\С#\Calculator\Calculator\Tessdata", "eng", EngineMode.Default))
            {
                engine.SetVariable("tessedit_char_whitelist", "0123456789+-=");

                using (var img = Pix.LoadFromFile("img.png"))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        if (text.Contains("="))
                        {
                            var equation = text.Split('=')[0].Trim();
                            var result = Calculate(equation);
                            MessageBox.Show(result.ToString(), "Результат вычисления");
                            _timer.Stop();
                        }
                    }
                }
            }
        }

        private double Calculate(string equation)
        {
            var elements = equation.Split(new[] { '+', '-' }, StringSplitOptions.RemoveEmptyEntries);
            double result = 0;

            foreach (var element in elements)
            {
                if (element.Contains("+"))
                {
                    var numbers = element.Split('+').Select(double.Parse);
                    result += numbers.Sum();
                }
                else if (element.Contains("-"))
                {
                    var numbers = element.Split('-').Select(double.Parse);
                    result -= numbers.Sum();
                }
                else
                {
                    result += double.Parse(element);
                }
            }

            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new MainWindow();
            newWindow.Width = 680;
            newWindow.Height = 775;

            var image = new Image();
            var fullFilePath = @"C:\Учеба\С#\Calculator\Calculator\Component\Images\about-prog.png";
            image.Source = new BitmapImage(new Uri(fullFilePath, UriKind.Absolute));

            var scrollViewer = new ScrollViewer();
            scrollViewer.Content = image;
            newWindow.Content = scrollViewer;

            newWindow.Show();
        }

    }
}