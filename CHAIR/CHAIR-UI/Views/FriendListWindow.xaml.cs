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
using System.Windows.Shapes;

namespace CHAIR_UI.Views
{
    /// <summary>
    /// Interaction logic for FriendListWindow.xaml
    /// </summary>
    public partial class FriendListWindow : Window
    {
        public FriendListWindow()
        {
            InitializeComponent();
            this.DataContext = new FriendListWindowViewModel();
        }

        /// <summary>
        /// TitleBar_MouseDown - Drag if single-click, resize if double-click
        /// </summary>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //Highlight the buttons as the mouse comes in
        private void TopButtons_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Button).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5C146D"));
        }

        //Give the button its normal color once the mouse leaves
        private void TopButtons_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Button).Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C0735"));
        }
    }
}
