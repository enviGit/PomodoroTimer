using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PomodoroTimer.Core;
using PomodoroTimer.Models;
using PomodoroTimer.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;

namespace PomodoroTimer.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly PomodoroEngine _timer = new();
        private readonly SessionService _sessionService = new();

        [ObservableProperty] private int _workMinutes = 25;
        [ObservableProperty] private int _breakMinutes = 5;

        [ObservableProperty] private bool _isSettingsOpen;
        [ObservableProperty] private string _timerText = "25:00";
        [ObservableProperty] private double _progressValue = 0;
        [ObservableProperty] private double _progressMax = 1;
        [ObservableProperty] private TaskbarItemProgressState _taskbarState = TaskbarItemProgressState.None;

        [ObservableProperty] private bool _isBreakMode;
        [ObservableProperty] private ObservableCollection<Session> _sessions = new();
        [ObservableProperty] private bool _isTimerRunning;

        public MainViewModel()
        {
            _timer.Tick += (s, time) =>
            {
                TimerText = time.ToString(@"mm\:ss");
                ProgressValue = _timer.TotalDuration.TotalSeconds - time.TotalSeconds;
            };

            _timer.Finished += (s, e) =>
            {
                IsTimerRunning = false;
                TaskbarState = TaskbarItemProgressState.Error;
                ProgressValue = ProgressMax;
                SystemSounds.Exclamation.Play();

                if (!IsBreakMode) CompleteSession();
                else StartWorkMode();
            };

            LoadData();
            UpdateTimerDisplay();
        }

        private async void LoadData()
        {
            Sessions = await _sessionService.LoadSessionsAsync();
            Sessions = new ObservableCollection<Session>(Sessions.OrderByDescending(s => s.EndTime));
        }


        [RelayCommand]
        private void ChangeSetting(string typeAndAmount)
        {
            var parts = typeAndAmount.Split(':');
            string type = parts[0];
            int amount = int.Parse(parts[1]);

            if (type == "Work")
            {
                WorkMinutes = Math.Clamp(WorkMinutes + amount, 5, 120);
                if (!IsBreakMode) UpdateTimerDisplay();
            }
            else if (type == "Break")
            {
                BreakMinutes = Math.Clamp(BreakMinutes + amount, 1, 60);
                if (IsBreakMode) UpdateTimerDisplay();
            }
        }

        [RelayCommand]
        private void ToggleTimer()
        {
            if (IsTimerRunning)
            {
                _timer.Pause();
                IsTimerRunning = false;
                TaskbarState = TaskbarItemProgressState.Paused;
            }
            else
            {
                IsSettingsOpen = false;

                if (!_timer.IsRunning && ProgressValue == 0)
                {
                    int minutes = IsBreakMode ? BreakMinutes : WorkMinutes;
                    _timer.Start(TimeSpan.FromMinutes(minutes));
                    ProgressMax = _timer.TotalDuration.TotalSeconds;
                }
                else
                {
                    _timer.Resume();
                }

                IsTimerRunning = true;
                TaskbarState = TaskbarItemProgressState.Normal;
            }
        }

        [RelayCommand]
        private void ToggleSettings()
        {
            IsSettingsOpen = !IsSettingsOpen;
        }

        [RelayCommand]
        private void Reset()
        {
            UpdateTimerDisplay();
        }

        private void UpdateTimerDisplay()
        {
            _timer.Stop();
            IsTimerRunning = false;
            TaskbarState = TaskbarItemProgressState.None;
            ProgressValue = 0;

            int minutes = IsBreakMode ? BreakMinutes : WorkMinutes;
            TimerText = $"{minutes:00}:00";
            ProgressMax = minutes * 60;
        }

        [RelayCommand]
        private async Task DeleteSession(Session session)
        {
            if (MessageBox.Show("Usunąć sesję?", "Potwierdzenie", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Sessions.Remove(session);
                await _sessionService.SaveSessionsAsync(Sessions);
            }
        }

        [RelayCommand]
        private async Task ClearAll()
        {
            if (MessageBox.Show("Usunąć wszystko?", "Potwierdzenie", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Sessions.Clear();
                await _sessionService.SaveSessionsAsync(Sessions);
            }
        }

        private async void CompleteSession()
        {
            var session = new Session(DateTime.Now, _timer.TotalDuration);
            Sessions.Insert(0, session);
            await _sessionService.SaveSessionsAsync(Sessions);

            IsBreakMode = true;
            UpdateTimerDisplay();
            MessageBox.Show("Dobra robota! Czas na przerwę.", "Pomodoro", MessageBoxButton.OK, MessageBoxImage.Information);
            TaskbarState = TaskbarItemProgressState.None;
        }

        private void StartWorkMode()
        {
            IsBreakMode = false;
            UpdateTimerDisplay();
            MessageBox.Show("Koniec przerwy! Wracamy do pracy.", "Pomodoro", MessageBoxButton.OK, MessageBoxImage.Information);
            TaskbarState = TaskbarItemProgressState.None;
        }
    }
}