using System;
using System.Globalization;

namespace Sharpen
{
	public class JavaCalendar
	{
		public static int HOUR_OF_DAY = 0;
		public static int MINUTE = 1;
		public static int SECOND = 2;
		public static int MILLISECOND = 3;
		public static int DATE = 4;
		public static int YEAR = 5;
		public static int MONTH = 6;
		public static int WEEK_OF_YEAR = 7;

		DateTime Time;

		public JavaCalendar ()
		{
			Time = DateTime.UtcNow;
		}

		public void Add (int type, int value)
		{
			switch (type) {
			case 0:
				Time.AddHours (value);
				break;
			case 1:
				Time.AddMinutes (value);
				break;
			case 2:
				Time.AddSeconds (value);
				break;
			case 3:
				Time.AddMilliseconds (value);
				break;
			case 5:
				Time.AddYears (value);
				break;
			case 6:
				Time.AddMonths (value);
				break;
			default:
				throw new NotSupportedException ();
			}
		}

		public JavaCalendar Clone ()
		{
			return (JavaCalendar) MemberwiseClone ();
		}

		public DateTime GetTime ()
		{
			return Time;
		}

		public void Set (int type, int value)
		{
			switch (type) {
			case 0:
				Time.AddHours (value - Time.Hour);
				break;
			case 1:
				Time.AddMinutes (value - Time.Minute);
				break;
			case 2:
				Time.AddSeconds (value - Time.Second);
				break;
			case 3:
				Time.AddMilliseconds (value - Time.Millisecond);
				break;
			case 5:
				Time.AddYears (value - Time.Year);
				break;
			case 6:
				Time.AddMonths (value - Time.Month);
				break;
			default:
				throw new NotSupportedException ();
			}
		}

		public void SetTimeInMillis (long milliseconds)
		{
			Time = new DateTime (1970, 0, 0).AddMilliseconds (milliseconds);
		}
	}

	public class JavaGregorianCalendar : JavaCalendar
	{
		public JavaGregorianCalendar (TimeZoneInfo timezone, CultureInfo culture)
		{
		}
	}
}

