using System;
using System.Globalization;
using System.Windows.Data;

namespace Alarm
{
	public sealed class RemainingTimeConverter
		: IValueConverter
	{
		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is TimeSpan))
				return "----";

			var remainingTime = (TimeSpan) value;
			if (remainingTime >= TimeSpan.FromHours(1))
			{
				var totalHours = (int)remainingTime.TotalHours;
				return Format(totalHours, "Stunde", "Stunden");
			}

			if (remainingTime >= TimeSpan.FromMinutes(1))
			{
				var totalMinutes = (int)remainingTime.TotalMinutes;
				return Format(totalMinutes, "Minute", "Minuten");
			}

			if (remainingTime >= TimeSpan.Zero)
			{
				var numberOfSeconds = (int)remainingTime.TotalSeconds;
				return Format(numberOfSeconds, "Sekunde", "Sekunden");
			}

			if (remainingTime > TimeSpan.FromMinutes(-1))
			{
				return "Bitte Probe entnehmen!";
			}

			var numberOfMinutes = -(int)remainingTime.TotalMinutes;
			return string.Format("Seit {0} überfällig. Bitte Probe sofort entnehmen", Format(numberOfMinutes, "Minute", "Minuten"));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion

		private static string Format(int number, string singular, string plural)
		{
			if (number == 1)
				return string.Format("1 {0}", singular);

			return string.Format("{0} {1}", number, plural);
		}
	}
}