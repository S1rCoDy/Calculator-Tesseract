using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using Tesseract;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        // ... остальной код ...

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_isTabPressed)
            {
                _isTabPressed = false;
                return;
            }

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)this.Width, (int)this.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(this);

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
    }
}
