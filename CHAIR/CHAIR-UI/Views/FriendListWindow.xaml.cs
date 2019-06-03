using CHAIR_UI.Interfaces;
using CHAIR_UI.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class FriendListWindow : Window, IBasicActionsFriendList
    {
        private ConversationWindow _convWindow;

        public FriendListWindow(object viewmodel)
        {
            InitializeComponent();
            this.DataContext = (ChairWindowViewModel)viewmodel;
            _convWindow = new ConversationWindow((ChairWindowViewModel)viewmodel);

            Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _convWindow.Close();
        }

        /// <summary>
        /// TitleBar_MouseDown - Drag if single-click, resize if double-click
        /// </summary>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    if (this.WindowState == WindowState.Maximized)
                        this.WindowState = WindowState.Normal;
                    else
                        this.WindowState = WindowState.Maximized;
                }
                else
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

        private void DoubleClickFriend(object sender, MouseButtonEventArgs e)
        {
            if(_convWindow.Visibility == Visibility.Hidden || _convWindow.Visibility == Visibility.Collapsed)
                _convWindow.Show();
        }

        private void MouseEnterFriendIcon(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void MouseLeaveFriendIcon(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        #region IBasicActionsFriendList Implementation
        public void OpenConversation()
        {
            if (_convWindow.Visibility == Visibility.Hidden || _convWindow.Visibility == Visibility.Collapsed)
                _convWindow.Show();
        }
        #endregion
    }
}
