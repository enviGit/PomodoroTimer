using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using PomodoroTimer.Models;

namespace PomodoroTimer.Services
{
    public class SessionService
    {
        private const string FileName = "sessions.json";

        public async Task SaveSessionsAsync(IEnumerable<Session> sessions)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            await using var stream = File.Create(FileName);
            await JsonSerializer.SerializeAsync(stream, sessions, options);
        }

        public async Task<ObservableCollection<Session>> LoadSessionsAsync()
        {
            if (!File.Exists(FileName))
                return new ObservableCollection<Session>();

            await using var stream = File.OpenRead(FileName);
            var list = await JsonSerializer.DeserializeAsync<List<Session>>(stream);
            return new ObservableCollection<Session>(list ?? new List<Session>());
        }
    }
}