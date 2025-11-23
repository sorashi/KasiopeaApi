using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using KasiopeaApi.Rest;
using KasiopeaApi.Rest.Model;

namespace KasiopeaApi
{
    public class KasiopeaInterface : IDisposable
    {
        private Client c;
        public KasiopeaInterface(string email, string password, string baseUrl = "https://kasiopea.matfyz.cz") {
            var uri = new Uri(baseUrl);
            c = new Client(new Uri(uri, "/api"), new ApiCredentials {
                Email = email,
                Password = password
            });
        }

        public KasiopeaInterface(Client c) {
            this.c = c;
        }

        private int? selectedTaskId;
        public async Task SelectTaskAsync(int year, CourseKind kind, char letter) {
            letter = char.ToUpper(letter);
            var courses = await c.CoursesGet(year);
            try {
                selectedTaskId = courses.Where(x => x.Year == year).Single(x => x.Kind == kind).Tasks.Single(x => x.Letter == letter.ToString())
                    .Id;
            }
            catch {
                throw new ArgumentException("Task not found");
            }
        }

        private Guid currentAttemptId;
        private StreamReader reader;
        public async Task<StreamReader> GetInputReaderAsync(Difficulty difficulty) {
            if(!selectedTaskId.HasValue) throw new InvalidOperationException("Task not selected");
            var attempt = await c.TaskPostNewAttempt(selectedTaskId.Value, difficulty);
            currentAttemptId = attempt.Id;
            var inputPath = await c.AttemptGetInput(attempt.Id);
            reader?.Dispose();
            reader = new StreamReader(inputPath);
            return reader;
        }

        private StreamWriter writer;
        private string outputPath;
        public StreamWriter GetOutputWriter() {
            this.writer?.Dispose();
            outputPath = Path.GetTempFileName();
            writer = new StreamWriter(outputPath);
            return writer;
        }

        public async Task<ApiAttemptState> PostOutputAsync() {
            if(currentAttemptId == default || outputPath == null) throw new InvalidOperationException("No attempt started");
            await writer.DisposeAsync();
            var attempt = await c.AttemptPostSubmit(currentAttemptId, outputPath);
            return attempt.State;
        }

        ~KasiopeaInterface() {
            ReleaseResources();
        }

        private void ReleaseResources() {
            reader?.Dispose();
            writer?.Dispose();
            try {
                c.AuthPostLogout().Wait();
            }
            catch {
                // ignore
            }
        }

        public void Dispose() {
            ReleaseResources();
            GC.SuppressFinalize(this);
        }
    }
}
