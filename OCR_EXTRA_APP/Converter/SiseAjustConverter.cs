using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;


namespace OCR_EXTRA_APP.converter
{
    internal class SiseAjustConverter : MarkupExtension, IValueConverter
    {
        private static SiseAjustConverter _instance;

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
            return _instance ??= new SiseAjustConverter();
        }
    }
}
