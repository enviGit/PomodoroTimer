using PomodoroTimer.Models;
using System;
using System.Windows;

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
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
        }
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Pause();
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Reset();
            UpdateTimeRemainingLabel();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTimeRemainingLabel();
        }
        private void UpdateTimeRemainingLabel()
        {
            TimeSpan remainingTime = timer.TimeRemaining;
            Dispatcher.Invoke(() => TimeRemainingLabel.Text = remainingTime.ToString("mm\\:ss"));
        }
    }
}
