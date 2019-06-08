using CHAIR_UI.ViewModels;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CHAIR_UI.UserControls
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsUC : UserControl
    {
        public SettingsUC(object viewmodel)
        {
            InitializeComponent();
            this.DataContext = (ChairWindowViewModel)viewmodel;
        }

        private void SettingsMouseEnterFolderIcon(object sender, MouseEventArgs e)
        {
            ((PackIcon)sender).Foreground = new SolidColorBrush(Colors.Gray);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void SettingsMouseLeaveFolderIcon(object sender, MouseEventArgs e)
        {
            ((PackIcon)sender).Foreground = new SolidColorBrush(Colors.Black);
            Mouse.OverrideCursor = null;
        }
    }
}
