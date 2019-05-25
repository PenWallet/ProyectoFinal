using CHAIR_UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CHAIR_UI.Converters
{
    class LibraryGameToButtonVisibilityInverse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                //string game = SharedInfo.installedGames.Single(x => x == (string)value);
                return Visibility.Collapsed;
            }
            catch (InvalidOperationException) { }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
