using ImageProcessing.Entities;
using ImageProcessing.UserControls;
using ImageProcessing.Windows;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using static ImageProcessing.Entities.ImageEffect;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;

namespace ImageProcessing.ViewModels
{
    public class ProcessingUserControlViewModel : INotifyPropertyChanged
    {
        private bool imageEffects;
        private float minimumHue;
        private float maximumHue;
        private float brightness;
        private float contrast;
        private float gamma;
        private bool showChanges;
        private bool grayScale;
        private bool replaceColor;
        private Color pixelColor;

        public MainWindow MainWindow;
        public readonly ProcessingUserControl UserControl;
        private readonly Manipulation manipulation;
        private readonly FileOperation fileOperation;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ImageEffect> Filters;

        public List<ImageEffect> EnabledFilters { get; set; }

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

                if (UserControl != null && UserControl.IsLoaded)
                    UserControl.rectangleGrayColor.Fill = new SolidColorBrush(value);
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

        public Bitmap ModifiedImage { get; set; }

        public Bitmap OriginalImage { get; set; }

        public EffectsWindow EffectsWindow { get; set; }

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

        public bool ImageEffects
        {
            get { return imageEffects; }
            set
            {
                imageEffects = value;
                OnPropertyChanged(nameof(ImageEffects));
            }
        }

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ProcessingUserControlViewModel(MainWindow mainWindow, ProcessingUserControl userControl)
        {
            EnabledFilters = new List<ImageEffect>();
            Filters = new List<ImageEffect>
            {
                { new ImageEffect(FilterType.Invert, "Invert Image") },
                { new ImageEffect(FilterType.SepiaTone, "Sepia Tone") },
                { new ImageEffect(FilterType.Emboss, "Emboss") },
                { new ImageEffect(FilterType.Emboss45Degree, "Emboss 45 Degree") },
                { new ImageEffect(FilterType.EmbossTopLeft, "Emboss Top Left") },
                { new ImageEffect(FilterType.EmbossIntense, "Emboss Intense") },
                { new ImageEffect(FilterType.Pixelate, "Pixelate", 1, 100, 1) },
                { new ImageEffect(FilterType.Median, "Median", 3, 19, 2) },
                { new ImageEffect(FilterType.BoxBlur, "Box Blur", 3, 19, 2) },
                { new ImageEffect(FilterType.GaussianBlur, "Gaussian Blur", 1, 100, 1) },
                { new ImageEffect(FilterType.EdgeDetection, "Edge Detection") },
                { new ImageEffect(FilterType.EdgeDetection45Degree, "Edge Detection 45 Degree") },
                { new ImageEffect(FilterType.EdgeDetectionHorizontal, "Edge Detection Horizontal") },
                { new ImageEffect(FilterType.EdgeDetectionVertical, "Edge Detection Vertical") },
                { new ImageEffect(FilterType.EdgeDetectionTopLeft, "Edge Detection Top Left") }
            };

            MaximumHue = 360;
            Contrast = 1;
            Gamma = 1;
            showChanges = true;
            PixelColor = Color.FromArgb(255, 255, 0, 0);
            imageEffects = true;

            MainWindow = mainWindow;
            UserControl = userControl;
            manipulation = new Manipulation();
            fileOperation = new FileOperation();

            manipulation.ImageFinished += OnImageFinished;
        }

        private void OnImageFinished(object sender, Manipulation.ImageEventArgs e)
        {
            UserControl.Dispatcher.Invoke(() =>
            {
                UserControl.rectangleManipulated.Visibility = Visibility.Hidden;
                UserControl.progressRingManipulated.Visibility = Visibility.Hidden;
                UserControl.progressRingManipulated.IsActive = false;
                UserControl.buttonModify.IsEnabled = true;

                ModifiedImage = e.Bitmap;
                DisplayImage(e.Bitmap, 2);
            });
        }

        public void DisplayImage(Bitmap bitmap, int window)
        {
            ImageSource source = fileOperation.BitmapToBitmapImage(bitmap);

            if (window == 1)
                UserControl.imageOriginal.Source = source;
            else if (window == 2)
                UserControl.imageManipulated.Source = source;
            else if (window == 3)
            {
                UserControl.imageOriginal.Source = source;
                UserControl.imageManipulated.Source = source;
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
            GrayScale = false;
            PixelColor = Color.FromArgb(255, 255, 0, 0);
            ReplaceColor = false;
            EnabledFilters.Clear();

            if (EffectsWindow != null)
                EffectsWindow.ViewModel.CreateFilterButtons(Filters, EnabledFilters);

            if (OriginalImage != null)
            {
                ModifiedImage = new Bitmap(OriginalImage);
                DisplayImage(ModifiedImage, 2);
            }
        }

        public void ToggleImages()
        {
            if (UserControl.borderManipulated.Visibility == Visibility.Visible)
            {
                UserControl.borderManipulated.Visibility = Visibility.Hidden;
                UserControl.borderOriginal.Visibility = Visibility.Visible;
            }
            else
            {
                UserControl.borderOriginal.Visibility = Visibility.Hidden;
                UserControl.borderManipulated.Visibility = Visibility.Visible;
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

            UserControl.rectangleColorMin.Fill = brushMin;
            UserControl.rectangleColorMax.Fill = brushMax;
        }

        public void ModifyImage()
        {
            UserControl.rectangleManipulated.Visibility = Visibility.Visible;
            UserControl.progressRingManipulated.IsActive = true;
            UserControl.progressRingManipulated.Visibility = Visibility.Visible;
            UserControl.buttonModify.IsEnabled = false;

            Thread thread = new Thread(() => manipulation.Modify(OriginalImage,
                                                                 MinimumHue,
                                                                 MaximumHue,
                                                                 Brightness,
                                                                 Contrast,
                                                                 Gamma,
                                                                 GrayScale,
                                                                 PixelColor,
                                                                 ReplaceColor,
                                                                 ImageEffects,
                                                                 EnabledFilters,
                                                                 Filters));
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

        public void ShowEffectsWindow(double top = -1, double left = -1)
        {
            if (EffectsWindow is null)
            {
                EffectsWindow = new EffectsWindow(this, top, left);
                EffectsWindow.Show();
            }
            else
                EffectsWindow.Focus();
        }
    }
}
