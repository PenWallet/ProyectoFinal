using CHAIR_UI.ViewModels;
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
    /// Interaction logic for Library.xaml
    /// </summary>
    public partial class Library : UserControl
    {
        public Library(object viewmodel)
        {
            InitializeComponent();
            this.DataContext = (ChairWindowViewModel)viewmodel;
        }

        private void MouseEnterFriendIcon(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void MouseLeaveFriendIcon(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }
    }
}
