using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomodoroTimer.Models
{
    public class Session
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCompleted { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan PausedTime { get; set; }
    }
}
