using Windows.UI.Xaml;

namespace Xblero.Frameworks.Stopwatchs
{
    public class DependencyTime : DependencyObject
    {
        public DependencyTime(int hour, int minute, int second)
        {
            Hour = hour;
            Minute = minute;
            Second = second;
        }

        public static readonly DependencyProperty HourProperty = DependencyProperty.Register(
            "Hour", typeof (int), typeof (DependencyTime), new PropertyMetadata(default(int)));

        public int Hour
        {
            get { return (int) GetValue(HourProperty); }
            set { SetValue(HourProperty, value); }
        }

        public static readonly DependencyProperty MinuteProperty = DependencyProperty.Register(
            "Minute", typeof (int), typeof (DependencyTime), new PropertyMetadata(default(int)));

        public int Minute
        {
            get { return (int) GetValue(MinuteProperty); }
            set { SetValue(MinuteProperty, value); }
        }

        public static readonly DependencyProperty SecondProperty = DependencyProperty.Register(
            "Second", typeof (int), typeof (DependencyTime), new PropertyMetadata(default(int)));

        public int Second
        {
            get { return (int) GetValue(SecondProperty); }
            set { SetValue(SecondProperty, value); }
        }
    }
}