using System.IO;

namespace Mp3
{
    class Logger
    {
        public void Log(StreamWriter file, string message)
        {
            file.WriteLine(message);
        }

        public void Log(StreamWriter file, string fileName, string artist, string title, string status)
        {
            var logRecord = $"status: {status}, fileName: {fileName}, title: {title}, artist: {artist}";

            file.WriteLine(logRecord);
        }
    }
}