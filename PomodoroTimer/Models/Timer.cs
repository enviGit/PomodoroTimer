using System;
using System.ComponentModel;
using System.Timers;

#nullable disable

namespace PomodoroTimer.Models
{
    public class Timer : INotifyPropertyChanged
    {
        private TimeSpan _timeRemaining;
        private bool _isRunning;
        private bool _isPaused;
        private TimeSpan _remainingTimeOnPause;
        public event PropertyChangedEventHandler PropertyChanged;
        private System.Timers.Timer _timer;
        public event EventHandler Tick;
        public TimeSpan Duration { get; }

        public Timer(TimeSpan duration)
        {
            Duration = duration;
            _timeRemaining = duration;
            _isRunning = false;
            _isPaused = false;
        }
        public TimeSpan TimeRemaining
        {
            get => _timeRemaining;
            private set
            {
                _timeRemaining = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeRemaining)));
            }
        }
        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                _isRunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
            }
        }
        public bool IsPaused
        {
            get => _isPaused;
            private set
            {
                _isPaused = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPaused)));
            }
        }
        public void Start()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            IsPaused = false;

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }
        public void Pause()
        {
            if (!IsRunning || IsPaused)
                return;

            if (_timer != null)
                _timer.Stop();

            IsRunning = false;
            IsPaused = true;
            _remainingTimeOnPause = TimeRemaining;
        }
        public void Reset()
        {
            if (IsRunning)
                _timer.Stop();

            IsRunning = false;
            IsPaused = false;
            _remainingTimeOnPause = TimeSpan.Zero;
            TimeRemaining = Duration;
        }
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            TimeRemaining -= TimeSpan.FromSeconds(1);

            if (TimeRemaining <= TimeSpan.Zero)
            {
                _timer.Stop();
                IsRunning = false;
                IsPaused = false;
            }

            if (IsPaused)
                _remainingTimeOnPause = TimeRemaining;

            OnTick();
        }
        private void OnTick()
        {
            if (Tick != null)
                Tick(this, EventArgs.Empty);
        }
    }
}