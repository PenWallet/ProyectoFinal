using CHAIR_Entities.Persistent;
using CHAIR_UI.Interfaces;
using CHAIR_UI.UserControls;
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
    /// Interaction logic for ChairWindow.xaml
    /// </summary>
    public partial class ChairWindow : Window, IBasicActionsChair
    {
        private FriendListWindow _friendListWindow;

        public ChairWindow()
        {
            InitializeComponent();
            this.DataContext = new ChairWindowViewModel(this);
            _friendListWindow = new FriendListWindow(this.DataContext);

            Closing += OnWindowClosing;
        }

        /// <summary>
        /// TitleBar_MouseDown - Drag if single-click, resize if double-click
        /// </summary>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
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
            else if (page == "Library")
                ContentCtrl.Content = new Library(viewmodel);
            else if (page == "Store")
                ContentCtrl.Content = new Store(viewmodel);
            else if (page == "Settings")
                ContentCtrl.Content = new Settings(viewmodel);
            else if (page == "Community")
                ContentCtrl.Content = new Search(viewmodel);
            else if(page == "Game")
                ContentCtrl.Content = new UserControls.Game(viewmodel);
            else if (page == "Admin")
                ContentCtrl.Content = new Admin(viewmodel);
            else if (page == "Log out")
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
            }
        }

        public void OpenConversation()
        {
            _friendListWindow.OpenConversation();
        }
        #endregion

        private void OpenFriendList_Click(object sender, RoutedEventArgs e)
        {
            _friendListWindow.Show();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            ((ChairWindowViewModel)DataContext).disconnectFromSignalR();
            ((ChairWindowViewModel)DataContext).dispose();

            _friendListWindow.Close();

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
