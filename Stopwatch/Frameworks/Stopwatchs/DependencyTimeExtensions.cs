using System;
using System.Globalization;

namespace Xblero.Frameworks.Stopwatchs
{
    public static class TimeExtensions
    {
        public static TimeSpan ToTimeSpan(this DependencyTime time)
        {
            return new TimeSpan(time.Hour, time.Minute, time.Second);
        }

        public static DependencyTime ToDependencyTime(this TimeSpan timeSpan)
        {
            return new DependencyTime(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        private static readonly TimeSpan HourTimeSpan = TimeSpan.FromHours(1);
        private static readonly TimeSpan NegativeHourTimeSpan = TimeSpan.FromHours(-1);

        public static string ToStopwatchString(this TimeSpan timeSpan)
        {
            if (timeSpan >= HourTimeSpan || timeSpan <= NegativeHourTimeSpan)
            {
                return timeSpan.ToString();
            }
            if (timeSpan >= TimeSpan.Zero)
            {
                return timeSpan.Minutes.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') + ":" +
                       timeSpan.Seconds.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
            }
            timeSpan = TimeSpan.Zero - timeSpan;
            return '-' + timeSpan.Minutes.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') + ":" +
                   timeSpan.Seconds.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
        }
    }
}