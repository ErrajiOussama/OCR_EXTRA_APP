using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace OCR_EXTRA_APP.converter
{
    internal class Convert_corriger_data : MarkupExtension, IValueConverter
    {
        private static Convert_corriger_data _instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) - System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ??= new Convert_corriger_data();
        }
    }
}
