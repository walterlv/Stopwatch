using System;
using System.Globalization;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Xblero.Frameworks.Stopwatchs;

namespace Xblero.Windows
{
    /// <summary>
    /// 表示计时器的主窗口。
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// 创建计时器的主窗口。
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            TimeListBox.SelectedIndex = 2;
            SecondTextBox.Focus();
            SecondTextBox.SelectAll();
        }

        #region Window 基本功能

        /// <summary>
        /// 拖动标题栏。
        /// </summary>
        private void TitlePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// 切换窗口最大化、最小化与正常模式。
        /// </summary>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (!IsLoaded) return;
            if (e.Property == WindowStateProperty)
            {
                if (WindowState == WindowState.Normal)
                {
                    TitlePanel.Visibility = Visibility.Visible;
                    WindowOperationPanel.Visibility = Visibility.Collapsed;
                    FrameBorder.BorderThickness = new Thickness(1);
                    LayoutRoot.Effect = WindowShadowEffect;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    TitlePanel.Visibility = Visibility.Collapsed;
                    WindowOperationPanel.Visibility = Visibility.Visible;
                    FrameBorder.BorderThickness = new Thickness(0);
                    LayoutRoot.Effect = null;
                }
            }
        }

        /// <summary>
        /// 禁止拖动滚动条时窗口抖动。
        /// </summary>
        protected override void OnManipulationBoundaryFeedback(ManipulationBoundaryFeedbackEventArgs e)
        {
        }

        /// <summary>
        /// 显示关于窗口。
        /// </summary>
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow window = new AboutWindow { Owner = this };
            window.ShowDialog();
        }

        /// <summary>
        /// 点击最大化按钮。
        /// </summary>
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    break;
                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    break;
            }
        }

        /// <summary>
        /// 点击最小化按钮。
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 点击关闭按钮。
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region 设置时间

        /// <summary>
        /// 监视窗口内焦点变换。
        /// 如果是设置时间，则显示时间；否则隐藏时间并修正输入。
        /// </summary>
        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox || e.OriginalSource is ListBoxItem)
            {
                TimeListAnimation.To = 0;
                TimeListStoryboard.Begin();
            }
            else
            {
                TimeListAnimation.To = TimeListPanel.ActualHeight;
                TimeListStoryboard.Begin();
                FixTimeInput();
            }
        }

        /// <summary>
        /// 监视时间项的选择。
        /// 将选择的时间设入输入框。
        /// </summary>
        private void TimeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DependencyTime time = (DependencyTime) TimeListBox.SelectedItem;
            SetTimeSpanToInput(time.ToTimeSpan());
        }

        /// <summary>
        /// 监视文本的输入。
        /// 只有有输入，则重置计时器。
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResetStopwatch();
        }

        #endregion

        #region 计时器实例

        /// <summary>
        /// 获取当前正在使用的计时器实例。
        /// </summary>
        private Stopwatch Stopwatch
        {
            get
            {
                if (_stopwatch == null)
                {
                    _stopwatch = new Stopwatch();
                    _stopwatch.RunningChanged += Stopwatch_RunningChanged;
                    _stopwatch.TimeChanged += Stopwatch_TimeChanged;
                }
                return _stopwatch;
            }
        }

        private Stopwatch _stopwatch;


        /// <summary>
        /// 获取或设置一个值。该值指示计时器处于工作状态（无论是开始还是暂停中）。反之则处于配置状态。
        /// </summary>
        private bool _isTiming;

        private static readonly TimeSpan WarningTimeSpan = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan DeadlineTimeSpan = TimeSpan.Zero;

        /// <summary>
        /// 当计时器的时间更新时，更新输出的时间文本。
        /// </summary>
        private void Stopwatch_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            SetTimeSpanToOutput(e.LeftTime);
        }

        /// <summary>
        /// 当计时器的状态更新时，更新界面状态。
        /// </summary>
        private void Stopwatch_RunningChanged(object sender, RunningChangedEventArgs e)
        {
            if (e.IsRunning)
            {
                SetTimeSpanToOutput(Stopwatch.LeftTime);
                StartButton.Style = PauseButtonStyle;
                ResetButton.Style = RestartButtonStyle;
            }
            else
            {
                StartButton.Style = ResumeButtonStyle;
                ResetButton.Style = ResetButtonStyle;
            }
        }

        #endregion

        #region 计时器操作按钮

        /// <summary>
        /// 开始、暂停、继续计时器。
        /// </summary>
        private void StartPauseButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.Focus();
            if (Stopwatch.IsRunning)
            {
                Stopwatch.Pause();
            }
            else
            {
                if (_isTiming)
                {
                    ResumeStopwatch();
                }
                else
                {
                    StartStopwatch();
                }
            }
        }

        /// <summary>
        /// 重置、重新开始计时器。
        /// </summary>
        private void ResetRestartButton_Click(object sender, RoutedEventArgs e)
        {
            if (Stopwatch.IsRunning)
            {
                ResetStopwatch();
                StartStopwatch();
            }
            else
            {
                ResetStopwatch();
            }
        }

        #endregion

        #region 计时操作

        /// <summary>
        /// 开始计时。
        /// </summary>
        private void StartStopwatch()
        {
            TimeSpan totalTimeSpan = GetTimeSpanFromInput();
            Stopwatch.TotalTime = totalTimeSpan;
            Stopwatch.Start();
            _isTiming = true;
        }

        /// <summary>
        /// 继续计时。
        /// </summary>
        private void ResumeStopwatch()
        {
            Stopwatch.Start();
        }

        /// <summary>
        /// 重置计时器。
        /// </summary>
        private void ResetStopwatch()
        {
            _isTiming = false;
            Stopwatch.Stop();
            StartButton.Style = StartButtonStyle;
            ResetButton.Style = ResetButtonStyle;
            SetTimeSpanToOutput(GetTimeSpanFromInput());
        }

        #endregion

        #region 输入输出

        /// <summary>
        /// 从用户输入获得时间。
        /// </summary>
        private TimeSpan GetTimeSpanFromInput()
        {
            int hour, minute, second;
            Int32.TryParse(HourTextBox.Text, out hour);
            Int32.TryParse(MinuteTextBox.Text, out minute);
            Int32.TryParse(SecondTextBox.Text, out second);
            return TimeSpan.FromHours(hour) + TimeSpan.FromMinutes(minute) +
                   TimeSpan.FromSeconds(second);
        }

        /// <summary>
        /// 修正用户输入的时间。
        /// </summary>
        private void FixTimeInput()
        {
            TimeSpan time = GetTimeSpanFromInput();
            SetTimeSpanToInput(time);
        }

        /// <summary>
        /// 将时间设回用户输入框。
        /// </summary>
        private void SetTimeSpanToInput(TimeSpan timeSpan)
        {
            HourTextBox.Text = timeSpan.Hours.ToString(CultureInfo.InvariantCulture);
            MinuteTextBox.Text = timeSpan.Minutes.ToString(CultureInfo.InvariantCulture);
            SecondTextBox.Text = timeSpan.Seconds.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 将时间设置到输出文本。
        /// </summary>
        private void SetTimeSpanToOutput(TimeSpan timeSpan)
        {
            DisplayTextBlock.Text = timeSpan.ToStopwatchString();
            if (timeSpan < DeadlineTimeSpan)
            {
                DisplayTextBlock.Style = DeadlineTimeStyle;
            }
            else if (timeSpan <= WarningTimeSpan)
            {
                DisplayTextBlock.Style = WarningTimeStyle;
                if (_stopwatch.IsRunning && timeSpan == DeadlineTimeSpan && File.Exists(DeadlineSoundFileName))
                {
                    DeadlinePlayer.Play();
                }
            }
            else
            {
                DisplayTextBlock.Style = NormalTimeStyle;
            }
        }

        #endregion
    }

    public partial class MainWindow
    {
        private Style _startButtonStyle;
        private Style _resumeButtonStyle;
        private Style _pauseButtonStyle;
        private Style _resetButtonStyle;
        private DoubleAnimation _timeListAnimation;
        private Storyboard _timeListStoryboard;
        private Style _deadlineTimeStyle;
        private Style _warningTimeStyle;
        private Style _normalTimeStyle;
        private Style _restartButtonStyle;
        private DropShadowEffect _windowShadowEffect;
        private SoundPlayer _deadlinePlayer;

        public Style StartButtonStyle
        {
            get { return _startButtonStyle ?? (_startButtonStyle = (Style) FindResource("StartButtonStyle")); }
        }

        public Style ResumeButtonStyle
        {
            get { return _resumeButtonStyle ?? (_resumeButtonStyle = (Style) FindResource("ResumeButtonStyle")); }
        }

        public Style PauseButtonStyle
        {
            get { return _pauseButtonStyle ?? (_pauseButtonStyle = (Style) FindResource("PauseButtonStyle")); }
        }

        public Style ResetButtonStyle
        {
            get { return _resetButtonStyle ?? (_resetButtonStyle = (Style) FindResource("ResetButtonStyle")); }
        }

        public Style RestartButtonStyle
        {
            get { return _restartButtonStyle ?? (_restartButtonStyle = (Style) FindResource("RestartButtonStyle")); }
        }

        public Style NormalTimeStyle
        {
            get { return _normalTimeStyle ?? (_normalTimeStyle = (Style) FindResource("NormalTimeStyle")); }
        }

        public Style WarningTimeStyle
        {
            get { return _warningTimeStyle ?? (_warningTimeStyle = (Style) FindResource("WarningTimeStyle")); }
        }

        public Style DeadlineTimeStyle
        {
            get { return _deadlineTimeStyle ?? (_deadlineTimeStyle = (Style) FindResource("DeadlineTimeStyle")); }
        }

        public Storyboard TimeListStoryboard
        {
            get
            {
                return _timeListStoryboard ?? (_timeListStoryboard = (Storyboard) FindResource("TimeListStoryboard"));
            }
        }

        public DoubleAnimation TimeListAnimation
        {
            get
            {
                return _timeListAnimation ?? (_timeListAnimation = (DoubleAnimation) TimeListStoryboard.Children[0]);
            }
        }

        public DropShadowEffect WindowShadowEffect
        {
            get
            {
                return _windowShadowEffect ??
                       (_windowShadowEffect = (DropShadowEffect) FindResource("WindowShadowEffect"));
            }
        }

        public SoundPlayer DeadlinePlayer
        {
            get { return _deadlinePlayer ?? (_deadlinePlayer = new SoundPlayer(DeadlineSoundFileName)); }
        }

        private const string DeadlineSoundFileName = "timeup.wav";
    }
}
