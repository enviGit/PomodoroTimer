using Newtonsoft.Json;
using PomodoroTimer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            TimeSpan duration = TimeSpan.FromMinutes(25);
            timer = new Timer(duration);
            timer.Tick += Timer_Tick;
            ProgressBar.Maximum = duration.TotalSeconds;

            if (File.Exists("sessions.json"))
            {
                string json = File.ReadAllText("sessions.json");
                var sessionListItems = JsonConvert.DeserializeObject<List<Session>>(json);

                if (sessionListItems == null) 
                    return;

                foreach (var sessionListItem in sessionListItems)
                {
                    var session = new Session
                    {
                        EndTime = DateTime.Parse(sessionListItem.EndTimeString),
                        Duration = TimeSpan.Parse(sessionListItem.DurationString)
                    };
                    Sessions.Add(session);
                }

                ClearSessionsButton.Visibility = Visibility.Visible;
                SessionList.ItemsSource = Sessions.OrderByDescending(x => x.EndTime);
                SessionHeaders.Visibility = Visibility.Visible;
                SessionList.Visibility = Visibility.Visible;
            }

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
            string json = JsonConvert.SerializeObject(Sessions, Formatting.Indented);
            File.WriteAllText("sessions.json", json);
            timer.Reset();
            UpdateTimeRemainingLabel();
            SessionHeaders.Visibility = Visibility.Visible;
            TimeRemainingLabel.Visibility = Visibility.Collapsed;
            ProgressBar.Visibility = Visibility.Collapsed;
            ProgressBar.Value = 0;
            currentSession = null;
            SessionList.ItemsSource = Sessions.OrderByDescending(x => x.EndTime);
            ClearSessionsButton.Visibility = Visibility.Visible;
            SessionList.Visibility = Visibility.Visible;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                bgImg.Effect = new BlurEffect { Radius = 20 };
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
                    string json = JsonConvert.SerializeObject(Sessions, Formatting.Indented);
                    File.WriteAllText("sessions.json", json);
                    timer.Reset();
                    SessionHeaders.Visibility = Visibility.Visible;
                    ProgressBar.Visibility = Visibility.Collapsed;
                    TimeRemainingLabel.Text = "";
                    currentSession = null;
                    SessionList.ItemsSource = Sessions.OrderByDescending(x => x.EndTime);
                    ClearSessionsButton.Visibility = Visibility.Visible;
                    SessionList.Visibility = Visibility.Visible;
                }
            });
        }
        private void UpdateTimeRemainingLabel()
        {
            TimeSpan remainingTime = timer.TimeRemaining;
            Dispatcher.Invoke(() => TimeRemainingLabel.Text = remainingTime.ToString("mm\\:ss"));
        }
        private void DeleteSessionButton_Click(object sender, RoutedEventArgs e)
        {
            var session = (sender as Button)?.DataContext as Session;

            if (session != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this session?", 
                    "Confirm deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Sessions.Remove(session);
                    string json = JsonConvert.SerializeObject(Sessions, Formatting.Indented);
                    File.WriteAllText("sessions.json", json);
                    SessionList.ItemsSource = Sessions.OrderByDescending(x => x.EndTime);
                }
            }
        }
        private void ClearSessionsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to permanently delete all your sessions? This action cannot be undone!", 
                "Confirm deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Sessions.Clear();
                File.WriteAllText("sessions.json", "");
                SessionList.ItemsSource = null;
            }
        }
    }
}