using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PomodoroTimer.Models
{
    public class Session
    {
        /*
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        */

        private const string DateTimeFormat = "dd/MM/yyyy HH:mm:ss";

        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }

        public string EndTimeString
        {
            get { return EndTime.ToString(DateTimeFormat); }
        }

        public string DurationString
        {
            get { return string.Format("{0:mm\\:ss}", Duration); }
        }
    }
}
