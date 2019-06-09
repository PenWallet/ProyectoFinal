using CHAIR_UI.Interfaces;
using CHAIR_UI.ViewModels;
using CHAIR_UI.Views;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CHAIR_UI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// ICloseable allows to be able to close the window whenever we want without breaking MVVM standards (it's an interface)
    /// </summary>
    public partial class LoginWindow : Window, IBasicActionsLogin
    {
        public LoginWindow()
        {
            this.InitializeComponent();
            this.DataContext = new LoginWindowViewModel(this);
        }

        /// <summary>
        /// TitleBar_MouseDown - Drag if single-click
        /// </summary>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount != 2)
                    this.DragMove();
        }

        //Just code to set the WindowStyle back to None after bringing the application back up
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (WindowStyle != WindowStyle.None)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (DispatcherOperationCallback)delegate (object unused)
                {
                    WindowStyle = WindowStyle.None;
                    return null;
                }
                , null);
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
            //Do nothing because we don't want to be able to maximize this window
            return;
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
                    if (!Application.Current.Windows.OfType<ChairWindow>().Any())
                    {
                        ChairWindow chairWindow = new ChairWindow();
                        chairWindow.Show();
                    }
                    break;

                case "RegisterWindow":
                    if (!Application.Current.Windows.OfType<RegisterWindow>().Any())
                    {
                        RegisterWindow regWindow = new RegisterWindow();
                        regWindow.Show();
                    }
                    break;
            }
        }

        public void SetPassword(string password)
        {
            Pass.Password = password;
        }
        #endregion

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(this.DataContext != null)
                ((LoginWindowViewModel)this.DataContext).password = ((PasswordBox)sender).Password;
        }
    }
}
