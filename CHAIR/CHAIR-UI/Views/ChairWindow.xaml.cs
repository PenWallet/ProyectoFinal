using CHAIR_Entities.Persistent;
using CHAIR_UI.Interfaces;
using CHAIR_UI.UserControls;
using CHAIR_UI.Utils;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CHAIR_UI.Views
{
    /// <summary>
    /// Interaction logic for ChairWindow.xaml
    /// </summary>
    public partial class ChairWindow : Window, IBasicActionsChair
    {
        private FriendListWindow _friendListWindow;
        private System.Windows.Forms.NotifyIcon _notifyIcon;

        public ChairWindow()
        {
            InitializeComponent();
            this.DataContext = new ChairWindowViewModel(this);
            _friendListWindow = new FriendListWindow(this.DataContext);

            Closing += OnWindowClosing;

            //Setup notifyIcon
            Image imageIcon = (Image)Application.Current.FindResource("CHAIRIcon");
            System.Drawing.Icon icon = Utilities.ConvertImageToIcon(imageIcon);

            //NotifyIcon
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.Icon = icon;
            _notifyIcon.Visible = false;
            _notifyIcon.DoubleClick += delegate (object sender, EventArgs args)
            {
                this.WindowState = WindowState.Normal;
                this.Show();
                _notifyIcon.Visible = false;
            };
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


        #region IBasicActionsChair Implementation
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
            throw new NotImplementedException();
        }

        public async void ShowPopUp(string message)
        {
            TextBlock view = new TextBlock()
            {
                Text = message,
                Margin = new Thickness(15, 10, 15, 10)
            };

            await DialogHost.Show(view, "ChairWindow");
        }

        public void ChangePage(string page, object viewmodel)
        {
            if (page == SharedInfo.loggedUser.nickname || page == "Profile")
                ContentCtrl.Content = new Profile(viewmodel);
            else
            {
                switch(page)
                {
                    case "Library":
                        ContentCtrl.Content = new Library(viewmodel);
                        break;
                    case "Store":
                        ContentCtrl.Content = new Store(viewmodel);
                        break;
                    case "Settings":
                        ContentCtrl.Content = new SettingsUC(viewmodel);
                        break;
                    case "Community":
                        ContentCtrl.Content = new Search(viewmodel);
                        break;
                    case "Game":
                        ContentCtrl.Content = new UserControls.Game(viewmodel);
                        break;
                    case "About":
                        ContentCtrl.Content = new About();
                        break;
                    case "Admin":
                        ContentCtrl.Content = new Admin();
                        break;
                    case "ProfileEdit":
                        ContentCtrl.Content = new ProfileEdit(viewmodel);
                        break;
                }
            }
        }

        public void OpenConversation()
        {
            _friendListWindow.OpenConversation();
        }

        public void MinimizeToTray()
        {
            this.Hide();
            _notifyIcon.Visible = true;
        }

        public void CloseWithParameter(bool callShutdown)
        {
            if(callShutdown)
                Application.Current.Shutdown();
            else
            {
                //Go back to the login window
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();

                //Close the application
                this.Close();
            }
        }
        #endregion

        private void OpenFriendList_Click(object sender, RoutedEventArgs e)
        {
            _friendListWindow.Show();
        }

        //We need to execute all of this code every time the application closes (*closes*, not minimizes nor goes to the tray)
        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            //If the user is playing something, then close the game because he decided to close our application. Fuck him
            if(SharedInfo.gameBeingPlayed != null)
                ((ChairWindowViewModel)DataContext).closeOpenGameIfOpen();
                
            //Close the friend list window
            _friendListWindow.Close();

            //Disconnect from SignalR
            ((ChairWindowViewModel)DataContext).disconnectFromSignalR();
            //Dispose of everything in the chair window viewmodel
            ((ChairWindowViewModel)DataContext).dispose();
        }
    }
}
