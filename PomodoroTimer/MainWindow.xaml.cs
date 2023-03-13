using PomodoroTimer.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

#nullable disable

namespace PomodoroTimer
{
    public partial class MainWindow : Window
    {
        private Timer timer;
        private ObservableCollection<Session> _sessions = new ObservableCollection<Session>();
        private Session currentSession;

        public MainWindow()
        {
            InitializeComponent();

            TimeSpan duration = TimeSpan.FromMinutes(25);
            timer = new Timer(duration);
            timer.Tick += Timer_Tick;
            ProgressBar.Maximum = duration.TotalSeconds;
        }
        public ObservableCollection<Session> Sessions
        {
            get { return _sessions; }
            set { _sessions = value; }
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsRunning)
                return;
            if (currentSession != null && !currentSession.IsCompleted)
                currentSession.PausedTime += currentSession.Duration - timer.TimeRemaining;

            timer.Start();
            TimeRemainingLabel.Visibility = Visibility.Visible;
        }
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Pause();
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentSession == null)
            {
                currentSession = new Session
                {
                    StartTime = DateTime.Now,
                    Duration = timer.Duration - timer.TimeRemaining,
                    PausedTime = TimeSpan.Zero
                };
                Sessions.Add(currentSession);
                SessionList.Visibility = Visibility.Visible;
            }

            timer.Reset();
            UpdateTimeRemainingLabel();
            TimeRemainingLabel.Visibility = Visibility.Collapsed;
            ProgressBar.Visibility = Visibility.Collapsed;
            ProgressBar.Value = 0;
            currentSession.EndTime = DateTime.Now;
            currentSession = null;
            SessionList.ItemsSource = Sessions;
            SessionList.Visibility = Visibility.Visible;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateTimeRemainingLabel();
                ProgressBar.Visibility = Visibility.Visible;
                ProgressBar.Value = (timer.Duration - timer.TimeRemaining).TotalSeconds;

                if (timer.TimeRemaining == TimeSpan.Zero)
                {
                    if (currentSession == null)
                    {
                        currentSession = new Session
                        {
                            StartTime = DateTime.Now,
                            Duration = timer.Duration,
                            PausedTime = TimeSpan.Zero
                        };
                        Sessions.Add(currentSession);
                        SessionList.Visibility = Visibility.Visible;
                    }

                    ProgressBar.Visibility = Visibility.Collapsed;
                    timer.Reset();
                    TimeRemainingLabel.Text = "";
                    currentSession.EndTime = DateTime.Now;
                    currentSession = null;
                    SessionList.ItemsSource = Sessions;
                    SessionList.Visibility = Visibility.Visible;
                }
            });
        }
        private void UpdateTimeRemainingLabel()
        {
            TimeSpan remainingTime = timer.TimeRemaining;
            Dispatcher.Invoke(() => TimeRemainingLabel.Text = remainingTime.ToString("mm\\:ss"));
        }
    }
}
