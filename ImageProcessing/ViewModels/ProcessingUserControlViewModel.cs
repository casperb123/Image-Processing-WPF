using ImageProcessing.Entities;
using ImageProcessing.UserControls;
using ImageProcessing.Windows;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;

namespace ImageProcessing.ViewModels
{
    public class ProcessingUserControlViewModel : INotifyPropertyChanged
    {
        private float minimumHue;
        private float maximumHue;
        private float brightness;
        private float contrast;
        private float gamma;
        private bool showChanges;
        private bool invert;
        private bool sepiaTone;
        private bool grayScale;
        private bool replaceColor;
        private int pixelateSize;
        private int medianSize;
        private bool pixelate;
        private bool medianFilter;
        private bool gaussianBlurFilter;
        private int gaussianBlurAmount;
        private bool boxBlurFilter;
        private int boxBlurAmount;
        private bool emboss;
        private Color pixelColor;

        private ConvolutionFilterBase filter;
        private ObservableCollection<ConvolutionFilterBase> filters;

        public MainWindow MainWindow;
        private ProcessingUserControl userControl;
        private Manipulation manipulation;
        private FileOperation fileOperation;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ShowChanges
        {
            get { return showChanges; }
            set
            {
                showChanges = value;
                OnPropertyChanged(nameof(ShowChanges));
                ToggleImages();
            }
        }

        public Color PixelColor
        {
            get { return pixelColor; }
            set
            {
                pixelColor = value;

                if (userControl != null && userControl.IsLoaded)
                    userControl.rectangleGrayColor.Fill = new SolidColorBrush(value);
            }
        }

        public bool Emboss
        {
            get { return emboss; }
            set
            {
                emboss = value;
                OnPropertyChanged(nameof(Emboss));
            }
        }

        public bool BlurFilters { get; set; }

        public bool BoxBlurFilter
        {
            get { return boxBlurFilter; }
            set
            {
                boxBlurFilter = value;
                OnPropertyChanged(nameof(BoxBlurFilter));
            }
        }

        public int BoxBlurAmount
        {
            get { return boxBlurAmount; }
            set
            {
                boxBlurAmount = value;
                OnPropertyChanged(nameof(BoxBlurAmount));
            }
        }

        public int GaussianBlurAmount
        {
            get { return gaussianBlurAmount; }
            set
            {
                gaussianBlurAmount = value;
                OnPropertyChanged(nameof(GaussianBlurAmount));
            }
        }

        public bool GaussianBlurFilter
        {
            get { return gaussianBlurFilter; }
            set
            {
                gaussianBlurFilter = value;
                OnPropertyChanged(nameof(GaussianBlurFilter));
            }
        }

        public ConvolutionFilterBase Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                OnPropertyChanged(nameof(Filter));
            }
        }

        public ObservableCollection<ConvolutionFilterBase> Filters
        {
            get { return filters; }
            private set
            {
                filters = value;
                OnPropertyChanged(nameof(Filters));
            }
        }

        public bool Pixelate
        {
            get { return pixelate; }
            set
            {
                pixelate = value;
                OnPropertyChanged(nameof(Pixelate));
            }
        }

        public bool MedianFilter
        {
            get { return medianFilter; }
            set
            {
                medianFilter = value;
                OnPropertyChanged(nameof(MedianFilter));
            }
        }

        public int MedianSize
        {
            get { return medianSize; }
            set
            {
                medianSize = value;
                OnPropertyChanged(nameof(MedianSize));
            }
        }

        public int PixelateSize
        {
            get { return pixelateSize; }
            set
            {
                pixelateSize = value;
                OnPropertyChanged(nameof(PixelateSize));
            }
        }

        public bool GrayScale
        {
            get { return grayScale; }
            set
            {
                grayScale = value;
                OnPropertyChanged(nameof(GrayScale));

                if (value)
                    ReplaceColor = false;
            }
        }

        public bool ReplaceColor
        {
            get { return replaceColor; }
            set
            {
                replaceColor = value;
                OnPropertyChanged(nameof(ReplaceColor));
            }
        }

        public bool SepiaTone
        {
            get { return sepiaTone; }
            set
            {
                sepiaTone = value;
                OnPropertyChanged(nameof(SepiaTone));
            }
        }

        public bool Invert
        {
            get { return invert; }
            set
            {
                invert = value;
                OnPropertyChanged(nameof(Invert));
            }
        }

        public Bitmap ModifiedImage { get; set; }

        public Bitmap OriginalImage { get; set; }

        public OtherEffectsWindow OtherEffectsWindow { get; set; }

        public float Gamma
        {
            get { return gamma; }
            set
            {
                gamma = value;
                OnPropertyChanged(nameof(Gamma));
            }
        }

        public float Contrast
        {
            get { return contrast; }
            set
            {
                contrast = value;
                OnPropertyChanged(nameof(Contrast));
            }
        }

        public float Brightness
        {
            get { return brightness; }
            set
            {
                brightness = value;
                OnPropertyChanged(nameof(Brightness));
            }
        }

        public float MaximumHue
        {
            get { return maximumHue; }
            set
            {
                maximumHue = value;
                OnPropertyChanged(nameof(MaximumHue));
            }
        }

        public float MinimumHue
        {
            get { return minimumHue; }
            set
            {
                minimumHue = value;
                OnPropertyChanged(nameof(MinimumHue));
            }
        }

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ProcessingUserControlViewModel(MainWindow mainWindow, ProcessingUserControl userControl)
        {
            MaximumHue = 360;
            Contrast = 1;
            Gamma = 1;
            showChanges = true;
            PixelateSize = 1;
            MedianSize = 3;
            GaussianBlurAmount = 1;
            BoxBlurAmount = 3;
            PixelColor = Color.FromArgb(255, 255, 0, 0);

            MainWindow = mainWindow;
            this.userControl = userControl;
            manipulation = new Manipulation();
            fileOperation = new FileOperation();

            manipulation.ImageFinished += OnImageFinished;
        }

        private void OnImageFinished(object sender, Manipulation.ImageEventArgs e)
        {
            userControl.Dispatcher.Invoke(() =>
            {
                userControl.rectangleManipulated.Visibility = Visibility.Hidden;
                userControl.progressRingManipulated.Visibility = Visibility.Hidden;
                userControl.progressRingManipulated.IsActive = false;
                userControl.buttonModify.IsEnabled = true;

                ModifiedImage = e.Bitmap;
                DisplayImage(e.Bitmap, 2);
            });
        }

        public void DisplayImage(Bitmap bitmap, int window)
        {
            ImageSource source = fileOperation.BitmapToBitmapImage(bitmap);

            if (window == 1)
                userControl.imageOriginal.Source = source;
            else if (window == 2)
                userControl.imageManipulated.Source = source;
            else if (window == 3)
            {
                userControl.imageOriginal.Source = source;
                userControl.imageManipulated.Source = source;
            }
        }

        public void OpenFile()
        {
            Bitmap bitmap = fileOperation.OpenFile();
            if (bitmap is null)
                return;

            OriginalImage = bitmap;
            ModifiedImage = bitmap;
            DisplayImage(OriginalImage, 3);
        }

        public async void ResetImage()
        {
            MessageDialogResult result = await MainWindow.ShowMessageAsync("Reset Image", "Are you sure that you want to reset the image?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Negative)
                return;

            MinimumHue = 0;
            MaximumHue = 360;
            Brightness = 0;
            Contrast = 1;
            Gamma = 1;
            Invert = false;
            SepiaTone = false;
            PixelateSize = 1;
            MedianSize = 3;
            GaussianBlurAmount = 1;
            BoxBlurAmount = 3;
            Pixelate = false;
            MedianFilter = false;
            GaussianBlurFilter = false;
            BoxBlurFilter = false;
            BlurFilters = false;
            GrayScale = false;
            PixelColor = Color.FromArgb(255, 255, 0, 0);
            ReplaceColor = false;

            if (OriginalImage != null)
            {
                ModifiedImage = new Bitmap(OriginalImage);
                DisplayImage(ModifiedImage, 2);
            }
        }

        public void ToggleImages()
        {
            if (userControl.borderManipulated.Visibility == Visibility.Visible)
            {
                userControl.borderManipulated.Visibility = Visibility.Hidden;
                userControl.borderOriginal.Visibility = Visibility.Visible;
            }
            else
            {
                userControl.borderOriginal.Visibility = Visibility.Hidden;
                userControl.borderManipulated.Visibility = Visibility.Visible;
            }
        }

        public void SetBoxColors()
        {
            System.Drawing.Color colorMin = manipulation.Hue(MinimumHue);
            System.Drawing.Color colorMax = manipulation.Hue(MaximumHue);
            Color brushColorMin = Color.FromArgb(colorMin.A, colorMin.R, colorMin.G, colorMin.B);
            Color brushColorMax = Color.FromArgb(colorMax.A, colorMax.R, colorMax.G, colorMax.B);
            Brush brushMin = new SolidColorBrush(brushColorMin);
            Brush brushMax = new SolidColorBrush(brushColorMax);

            userControl.rectangleColorMin.Fill = brushMin;
            userControl.rectangleColorMax.Fill = brushMax;
        }

        public void ModifyImage()
        {
            userControl.rectangleManipulated.Visibility = Visibility.Visible;
            userControl.progressRingManipulated.IsActive = true;
            userControl.progressRingManipulated.Visibility = Visibility.Visible;
            userControl.buttonModify.IsEnabled = false;

            Thread thread = new Thread(() => manipulation.Modify(OriginalImage,
                                                                 MinimumHue,
                                                                 MaximumHue,
                                                                 Brightness,
                                                                 Contrast,
                                                                 Gamma,
                                                                 GrayScale,
                                                                 PixelColor,
                                                                 ReplaceColor,
                                                                 Invert,
                                                                 SepiaTone,
                                                                 Pixelate,
                                                                 PixelateSize,
                                                                 MedianFilter,
                                                                 MedianSize,
                                                                 BlurFilters,
                                                                 GaussianBlurFilter,
                                                                 GaussianBlurAmount,
                                                                 BoxBlurFilter,
                                                                 BoxBlurAmount,
                                                                 Emboss));
            thread.Start();
        }

        public async void SaveImage()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Image File (*.png)|*.png|Image File (*.jpg)|*.jpg|Image File (*.bmp)|*.bmp";

            if (dialog.ShowDialog() == true)
            {
                FileStream stream = new FileStream(dialog.FileName, FileMode.Create);
                string format = Path.GetExtension(dialog.FileName);

                if (format == ".png")
                    ModifiedImage.Save(stream, ImageFormat.Png);
                else if (format == ".jpg")
                    ModifiedImage.Save(stream, ImageFormat.Jpeg);
                else if (format == ".bmp")
                    ModifiedImage.Save(stream, ImageFormat.Bmp);

                stream.Close();
                await MainWindow.ShowMessageAsync("Image Saved", "The image has been successfully saved!");
            }
        }
    }
}
