using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace Xblero.Frameworks.Stopwatchs
{
    public class Stopwatch
    {
        /// <summary>
        /// 获取预定义的时间。
        /// </summary>
        public static ObservableCollection<DependencyTime> PredefinedTimeSpan = new ObservableCollection<DependencyTime>
        {
            TimeSpan.FromSeconds(15).ToDependencyTime(),
            TimeSpan.FromSeconds(30).ToDependencyTime(),
            TimeSpan.FromSeconds(45).ToDependencyTime(),
            TimeSpan.FromMinutes(1).ToDependencyTime(),
            TimeSpan.FromSeconds(90).ToDependencyTime(),
            TimeSpan.FromMinutes(2).ToDependencyTime(),
            TimeSpan.FromMinutes(5).ToDependencyTime(),
            TimeSpan.FromMinutes(10).ToDependencyTime(),
            TimeSpan.FromMinutes(15).ToDependencyTime(),
            TimeSpan.FromMinutes(30).ToDependencyTime(),
            TimeSpan.FromMinutes(45).ToDependencyTime(),
            TimeSpan.FromHours(1).ToDependencyTime(),
        };

        public bool IsRunning
        {
            get { return Timer.IsEnabled; }
            private set
            {
                if (Timer.IsEnabled) Timer.Stop();
                else Timer.Start();
                OnRunningChanged(new RunningChangedEventArgs(value));
            }
        }

        public event EventHandler<RunningChangedEventArgs> RunningChanged;
        public event EventHandler<TimeChangedEventArgs> TimeChanged;

        public TimeSpan TotalTime
        {
            get { return _totalTime; }
            set
            {
                ThrowIfRunning();
                _totalTime = value;
                LeftTime = value;
            }
        }

        public TimeSpan LeftTime { get; private set; }

        public void Start()
        {
            if (!IsRunning)
            {
                _timer.Start();
                OnRunningChanged(new RunningChangedEventArgs(true));
            }
        }

        public void Pause()
        {
            if (IsRunning)
            {
                IsRunning = false;
                OnRunningChanged(new RunningChangedEventArgs(false));
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                _timer.Stop();
                OnRunningChanged(new RunningChangedEventArgs(false));
            }
        }

        #region 私有属性和方法

        private DispatcherTimer Timer
        {
            get
            {
                if (_timer == null)
                {
                    _timer = new DispatcherTimer {Interval = StepTime};
                    _timer.Tick += Timer_Tick;
                }
                return _timer;
            }
        }

        private DispatcherTimer _timer;
        private static readonly TimeSpan StepTime = TimeSpan.FromSeconds(1);
        private TimeSpan _totalTime;

        private void Timer_Tick(object sender, object o)
        {
            LeftTime -= StepTime;
            OnTimeChanged(new TimeChangedEventArgs(TotalTime, LeftTime));
        }

        private void OnTimeChanged(TimeChangedEventArgs e)
        {
            EventHandler<TimeChangedEventArgs> handler = TimeChanged;
            if (handler != null) handler(this, e);
        }

        private void OnRunningChanged(RunningChangedEventArgs e)
        {
            EventHandler<RunningChangedEventArgs> handler = RunningChanged;
            if (handler != null) handler(this, e);
        }

        private void ThrowIfRunning()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Invalid operation when the stopwatch is running.");
            }
        }

        #endregion
    }

    public class TimeChangedEventArgs : EventArgs
    {

        public TimeSpan TotalTime { get; private set; }
        public TimeSpan LeftTime { get; private set; }

        public TimeChangedEventArgs(TimeSpan totalTime, TimeSpan leftTime)
        {
            TotalTime = totalTime;
            LeftTime = leftTime;
        }
    }

    public class RunningChangedEventArgs : EventArgs
    {
        public bool IsRunning { get; private set; }

        public RunningChangedEventArgs(bool isRunning)
        {
            IsRunning = isRunning;
        }
    }
}
