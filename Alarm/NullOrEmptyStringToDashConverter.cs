using System;
using System.Globalization;
using System.Windows.Data;

namespace Alarm
{
	public sealed class NullOrEmptyStringToDashConverter
		: IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			const string dashes = "----";
			if (value == null)
				return dashes;

			var tmp = value as string;
			if (tmp == null)
				return null;

			if (string.IsNullOrEmpty(tmp))
				return dashes;

			return tmp;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}