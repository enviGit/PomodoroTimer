using Newtonsoft.Json;
using PomodoroTimer.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Effects;
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

            if (File.Exists("sessions.json"))
            {
                string json = File.ReadAllText("sessions.json");
                var sessionListItems = JsonConvert.DeserializeObject<List<Session>>(json);

                foreach (var sessionListItem in sessionListItems)
                {
                    var session = new Session
                    {
                        EndTime = DateTime.Parse(sessionListItem.EndTimeString),
                        Duration = TimeSpan.Parse(sessionListItem.DurationString)
                    };
                    Sessions.Add(session);
                }

                SessionList.ItemsSource = Sessions;
                SessionHeaders.Visibility = Visibility.Visible;
                SessionList.Visibility = Visibility.Visible;
            }

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

            timer.Start();
            TimeRemainingLabel.Visibility = Visibility.Visible;
        }
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Pause();
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            currentSession = new Session
            {
                EndTime = DateTime.Now,
                Duration = timer.Duration - timer.TimeRemaining
            };

            if (currentSession.Duration != TimeSpan.Zero)
                Sessions.Add(currentSession);

            bgImg.Effect = new BlurEffect { Radius = 0 };
            bgImg.Opacity = 1;
            string json = JsonConvert.SerializeObject(Sessions, Formatting.Indented);
            File.WriteAllText("sessions.json", json);
            timer.Reset();
            UpdateTimeRemainingLabel();
            SessionHeaders.Visibility = Visibility.Visible;
            SessionList.Visibility = Visibility.Visible;
            TimeRemainingLabel.Visibility = Visibility.Collapsed;
            ProgressBar.Visibility = Visibility.Collapsed;
            ProgressBar.Value = 0;
            currentSession = null;
            SessionList.ItemsSource = Sessions;
            SessionList.Visibility = Visibility.Visible;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                bgImg.Effect = new BlurEffect { Radius = 20 };
                bgImg.Opacity = 0.8;
                UpdateTimeRemainingLabel();
                ProgressBar.Visibility = Visibility.Visible;
                ProgressBar.Value = (timer.Duration - timer.TimeRemaining).TotalSeconds;

                if (timer.TimeRemaining == TimeSpan.Zero)
                {
                    currentSession = new Session
                    {
                        EndTime = DateTime.Now,
                        Duration = timer.Duration
                    };
                    Sessions.Add(currentSession);
                    bgImg.Effect = new BlurEffect { Radius = 0 };
                    bgImg.Opacity = 1;
                    string json = JsonConvert.SerializeObject(Sessions, Formatting.Indented);
                    File.WriteAllText("sessions.json", json);
                    timer.Reset();
                    SessionHeaders.Visibility = Visibility.Visible;
                    SessionList.Visibility = Visibility.Visible;
                    ProgressBar.Visibility = Visibility.Collapsed;
                    TimeRemainingLabel.Text = "";
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