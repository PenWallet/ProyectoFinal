using CHAIR_UI.Interfaces;
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
    /// Interaction logic for ProfileEdit.xaml
    /// </summary>
    public partial class ProfileEdit : UserControl, IPasswordBox
    {
        public ProfileEdit(object viewmodel)
        {
            InitializeComponent();
            this.DataContext = new ProfileEditUserControlViewModel(this, ((ChairWindowViewModel)viewmodel).notificationsQueue);
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
                ((ProfileEditUserControlViewModel)this.DataContext).profileEditPassword = ((PasswordBox)sender).Password;
        }

        public void SetPassword(string password)
        {
            Pass.Password = password;
        }
    }
}
