using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CHAIR_UI.Converters
{
    class AccCreatDateToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            DateTime now = DateTime.Now;
            double days = now.Subtract(date).TotalDays;
            string attachment = "";
            if (days < 7)
                attachment = $"{(int)days} days";
            else if (days < 30)
            {
                int weeks = (int)days / 7;
                attachment = $"{weeks} {(weeks == 1 ? "week" : "weeks")}";
            }
            else if (days < 365.25)
            {
                int months = (int)days / 30;
                attachment = $"{months} {(months == 1 ? "month" : "months")}";
            }
            else
            {
                int years = (int)(days / 365.25);
                attachment = $"{years} {(years == 1 ? "year" : "years")}";
            }
                
            return $"Member since {date.ToShortDateString()} ({attachment} old)";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
