using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PomodoroTimer.Core;
using PomodoroTimer.Models;
using PomodoroTimer.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PomodoroTimer.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly PomodoroEngine _timer = new();
        private readonly SessionService _sessionService = new();

        [ObservableProperty] private string _timerText = "25:00";
        [ObservableProperty] private double _progressValue = 0;
        [ObservableProperty] private double _progressMax = 1;
        [ObservableProperty] private bool _isBreakMode;
        [ObservableProperty] private ObservableCollection<Session> _sessions = new();
        [ObservableProperty] private bool _isTimerRunning;

        // Konstruktor
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
                if (!IsBreakMode) CompleteSession();
                else StartWorkMode();
            };

            LoadData();
        }

        private async void LoadData()
        {
            Sessions = await _sessionService.LoadSessionsAsync();
            Sessions = new ObservableCollection<Session>(Sessions.OrderByDescending(s => s.EndTime));
        }

        [RelayCommand]
        private void ToggleTimer()
        {
            if (IsTimerRunning)
            {
                _timer.Pause();
                IsTimerRunning = false;
            }
            else
            {
                if (_timer.TotalDuration == TimeSpan.Zero || _timerText == "25:00" && !_timer.IsRunning)
                    _timer.Start(TimeSpan.FromMinutes(IsBreakMode ? 5 : 25));
                else
                    _timer.Resume();

                ProgressMax = _timer.TotalDuration.TotalSeconds;
                IsTimerRunning = true;
            }
        }

        [RelayCommand]
        private void Reset()
        {
            _timer.Stop();
            IsTimerRunning = false;
            TimerText = IsBreakMode ? "05:00" : "25:00";
            ProgressValue = 0;
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
            TimerText = "05:00";
            ProgressValue = 0;
            ProgressMax = 300;

            MessageBox.Show("Czas na przerwę!", "Koniec pracy", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StartWorkMode()
        {
            IsBreakMode = false;
            TimerText = "25:00";
            ProgressValue = 0;
            MessageBox.Show("Wracamy do pracy!", "Koniec przerwy", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}