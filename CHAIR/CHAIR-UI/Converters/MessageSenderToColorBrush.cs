using CHAIR_UI.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CHAIR_UI.Converters
{
    class MessageSenderToColorBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(((string)value) == SharedInfo.loggedUser.nickname)
                return "#77DD77"; //Pastel Green
            else
                return "#779ECB"; //Pastel Blue
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
