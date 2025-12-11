using System;
using System.Windows.Threading;

namespace PomodoroTimer.Core
{
    public class PomodoroEngine
    {
        private readonly DispatcherTimer _timer;
        private TimeSpan _timeLeft;

        public event EventHandler<TimeSpan> Tick;
        public event EventHandler Finished;

        public TimeSpan TotalDuration { get; private set; }
        public bool IsRunning => _timer.IsEnabled;

        public PomodoroEngine()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => OnTick();
        }

        public void Start(TimeSpan duration)
        {
            TotalDuration = duration;
            _timeLeft = duration;
            _timer.Start();
            Tick?.Invoke(this, _timeLeft);
        }

        public void Resume()
        {
            if (_timeLeft > TimeSpan.Zero) _timer.Start();
        }

        public void Pause() => _timer.Stop();

        public void Stop()
        {
            _timer.Stop();
            _timeLeft = TotalDuration;
            Tick?.Invoke(this, _timeLeft);
        }

        private void OnTick()
        {
            _timeLeft = _timeLeft.Subtract(TimeSpan.FromSeconds(1));
            Tick?.Invoke(this, _timeLeft);

            if (_timeLeft <= TimeSpan.Zero)
            {
                _timer.Stop();
                Finished?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}