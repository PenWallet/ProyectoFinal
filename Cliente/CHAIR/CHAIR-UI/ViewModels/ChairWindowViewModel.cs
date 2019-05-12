using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CHAIR_UI.ViewModels
{
    public class ChairWindowViewModel : VMBase
    {
        #region Constructors
        public ChairWindowViewModel()
        {
            closeCommand = new RelayCommand<Window>(CloseWindow);
            minimizeCommand = new RelayCommand<Window>(MinimizeCommand);
        }
        #endregion


        #region Private properties

        #endregion


        #region Public properties
        public RelayCommand<Window> closeCommand { get; private set; }
        public RelayCommand<Window> minimizeCommand { get; private set; }
        #endregion

        #region Commands
        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                //Close this window
                window.Close();
            }

        }

        private void MinimizeCommand(Window window)
        {
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }
        #endregion
    }
}
