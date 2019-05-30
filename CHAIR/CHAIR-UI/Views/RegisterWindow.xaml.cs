using CHAIR_UI.Interfaces;
using CHAIR_UI.ViewModels;
using MaterialDesignThemes.Wpf;
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
using System.Windows.Threading;

namespace CHAIR_UI.Views
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window, IBasicActionsRegister
    {
        public RegisterWindow()
        {
            InitializeComponent();
            this.DataContext = new RegisterWindowViewModel(this);
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
                    Maximize();
                }
                else
                    this.DragMove();
            }
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

        #region IBasicActions implementation
        public void Maximize()
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        public void Minimize()
        {
            this.WindowState = WindowState.Minimized;
        }

        public void OpenWindow(string window)
        {
            switch (window)
            {
                case "ChairWindow":
                    ChairWindow chairWindow = new ChairWindow();
                    chairWindow.Show();
                    break;

                case "LoginWindow":
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    break;
            }
        }

        public async void ShowPopUp(string message)
        {
            TextBlock view = new TextBlock()
            {
                Text = message,
                Margin = new Thickness(15, 10, 15, 10)
            };

            await DialogHost.Show(view, "RegisterDialog");
        }

        public async void ShowPopUpAndLogin()
        {
            TextBlock view = new TextBlock()
            {
                Text = "Registered succesfully!",
                Margin = new Thickness(15, 10, 15, 10)
            };

            await DialogHost.Show(view, "RegisterDialog", closingEvent);
        }

        private void closingEvent(object sender, DialogClosingEventArgs eventArgs)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();

            this.Close();
        }
        #endregion
    }
}
