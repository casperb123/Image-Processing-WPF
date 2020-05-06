using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            ImageSource source = fileOperation.BitmapToBitmapImage(bitmap);

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

        private void ButtonResetImage_Click(object sender, RoutedEventArgs e)
        {
            modifiedImage = new Bitmap(originalImage);
            DisplayImage(modifiedImage, 2);
        }

        private void ButtonToggleImages_Click(object sender, RoutedEventArgs e)
        {
            if (borderManipulated.Visibility == Visibility.Visible)
            {
                borderManipulated.Visibility = Visibility.Hidden;
                borderOriginal.Visibility = Visibility.Visible;
                buttonToggleImages.Content = "Show Manipulated";
            }
            else
            {
                borderOriginal.Visibility = Visibility.Hidden;
                borderManipulated.Visibility = Visibility.Visible;
                buttonToggleImages.Content = "Show Original";
            }
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

        private async void ButtonSaveImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp";
            
            if (dialog.ShowDialog() == true)
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageManipulated.Source));

                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                {
                    encoder.Save(stream);
                }

                await this.ShowMessageAsync("Image Saved", "The image has been successfully saved!");
            }
        }
    }
}
