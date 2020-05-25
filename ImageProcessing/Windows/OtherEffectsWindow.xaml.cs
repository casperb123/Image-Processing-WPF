using ImageProcessing.ViewModels;
using MahApps.Metro.Controls;
using System.ComponentModel;

namespace ImageProcessing.Windows
{
    /// <summary>
    /// Interaction logic for PixelateSizeWindow.xaml
    /// </summary>
    public partial class OtherEffectsWindow : MetroWindow
    {
        public OtherEffectsWindowViewModel ViewModel;

        public OtherEffectsWindow(ProcessingUserControlViewModel processingUserControlViewModel)
        {
            InitializeComponent();
            ViewModel = new OtherEffectsWindowViewModel(processingUserControlViewModel);
            DataContext = ViewModel;
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            ViewModel.ProcessingUserControlViewModel.OtherEffectsWindow = null;
        }
    }
}
