using MahApps.Metro.Controls;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;

namespace ImageProcessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Bitmap originalImage;
        private Bitmap modifiedImage;
        private Bitmap[,] imageArray;
        private Bitmap[,] modifiedImageArray;

        private double minimum;
        private double maximum;

        private Manipulation manipulation;
        private FileOperation fileOperation;

        public MainWindow()
        {
            InitializeComponent();
            manipulation = new Manipulation();
            fileOperation = new FileOperation();

            manipulation.ImageFinished += OnImageFinished;
        }

        private void DisplayImage(Bitmap bitmap, int window)
        {
            ImageSource source = fileOperation.BitmapToImageSource(bitmap);

            if (window == 1)
                imageOriginal.Source = source;
            else if (window == 2)
                imageManipulated.Source = source;
            else if (window == 3)
            {
                imageOriginal.Source = source;
                imageManipulated.Source = source;
            }
        }

        private void OnImageFinished(object sender, Manipulation.ImageEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                modifiedImage = e.Bitmap;
                DisplayImage(e.Bitmap, 2);
            });
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            Bitmap bitmap = fileOperation.OpenFile();
            if (bitmap is null)
                return;

            originalImage = bitmap;
            DisplayImage(originalImage, 3);
        }

        private void SliderHue_ValueChanged(object sender, RangeParameterChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            minimum = sliderHue.LowerValue;
            maximum = sliderHue.UpperValue;

            System.Drawing.Color colorMin = manipulation.Hue(sliderHue.LowerValue);
            System.Drawing.Color colorMax = manipulation.Hue(sliderHue.UpperValue);
            System.Windows.Media.Color brushColorMin = System.Windows.Media.Color.FromArgb(colorMin.A, colorMin.R, colorMin.G, colorMin.B);
            System.Windows.Media.Color brushColorMax = System.Windows.Media.Color.FromArgb(colorMax.A, colorMax.R, colorMax.G, colorMax.B);
            Brush brushMin = new SolidColorBrush(brushColorMin);
            Brush brushMax = new SolidColorBrush(brushColorMax);

            rectangleColorMin.Fill = brushMin;
            rectangleColorMax.Fill = brushMax;
        }

        private void ButtonModify_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(() => manipulation.Modify(originalImage, minimum, maximum));
            thread.Start();
        }
    }
}
