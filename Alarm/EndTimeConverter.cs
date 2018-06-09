using System;
using System.Globalization;
using System.Windows.Data;

namespace Alarm
{
	public sealed class EndTimeConverter
		: IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is DateTime))
				return "----";

			var endTime = ((DateTime) value).ToLocalTime();
			return endTime.ToString("t");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}