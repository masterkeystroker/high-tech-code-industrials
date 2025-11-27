using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace CretaBase
{
    /// Converts Boolean Values to Control.Visibility values
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        private Visibility valueWhenTrue = Visibility.Visible;
        public Visibility ValueWhenTrue
        {
            get { return valueWhenTrue; }
            set { valueWhenTrue = value; }
        }

        private Visibility valueWhenFalse = Visibility.Collapsed;
        public Visibility ValueWhenFalse
        {
            get { return valueWhenFalse; }
            set { valueWhenFalse = value; }
        }

        private Visibility valueWhenNull = Visibility.Hidden;
        public Visibility ValueWhenNull
        {
            get { return valueWhenNull; }
            set { valueWhenNull = value; }
        }

        private object GetVisibility(object value)
        {
            if (!(value is bool) || value == null)
                return ValueWhenNull;

            if ((bool)value)
                return valueWhenTrue;

            return valueWhenFalse;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetVisibility(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
