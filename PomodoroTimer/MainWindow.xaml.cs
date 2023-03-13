using PomodoroTimer.Models;
using System;
using System.Windows;
using System.Windows.Threading;

#nullable disable

namespace PomodoroTimer
{
    public partial class MainWindow : Window
    {
        private Timer timer;

        public MainWindow()
        {
            InitializeComponent();

            TimeSpan duration = TimeSpan.FromMinutes(25);
            timer = new Timer(duration);
            timer.Tick += Timer_Tick;
            ProgressBar.Maximum = duration.TotalSeconds;
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            TimeRemainingLabel.Visibility = Visibility.Visible;
        }
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Pause();
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Reset();
            UpdateTimeRemainingLabel();
            TimeRemainingLabel.Visibility = Visibility.Collapsed;
            ProgressBar.Visibility = Visibility.Collapsed;
            ProgressBar.Value = 0;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => 
            {
                UpdateTimeRemainingLabel();
                ProgressBar.Visibility = Visibility.Visible;
                ProgressBar.Value = (timer.Duration - timer.TimeRemaining).TotalSeconds;

                if (timer.TimeRemaining == TimeSpan.Zero)
                    ProgressBar.Visibility = Visibility.Collapsed;
            });
        }
        private void UpdateTimeRemainingLabel()
        {
            TimeSpan remainingTime = timer.TimeRemaining;
            Dispatcher.Invoke(() => TimeRemainingLabel.Text = remainingTime.ToString("mm\\:ss"));
        }
    }
}
