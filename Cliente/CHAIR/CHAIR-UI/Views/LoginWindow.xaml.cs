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
    public partial class LoginWindow : Window, IBasicActions
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
                    ChairWindow chairWindow = new ChairWindow();
                    chairWindow.Show();
                    break;

                case "RegisterWindow":
                    RegisterWindow regWindow = new RegisterWindow();
                    regWindow.Show();
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

            await DialogHost.Show(view, "LoginDialog", closingHandler);
        }

        private void closingHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            
        }
        #endregion

    }
}
