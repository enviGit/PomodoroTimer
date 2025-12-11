using System;

namespace PomodoroTimer.Models
{
    public record Session(DateTime EndTime, TimeSpan Duration)
    {
        public string EndTimeString => EndTime.ToString("dd/MM/yyyy HH:mm");
        public string DurationString => Duration.ToString(@"hh\:mm\:ss");
    }
}