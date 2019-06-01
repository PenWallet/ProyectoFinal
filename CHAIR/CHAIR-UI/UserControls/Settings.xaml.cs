using CHAIR_UI.ViewModels;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CHAIR_UI.UserControls
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings(object viewmodel)
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
