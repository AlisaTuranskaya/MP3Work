using System;
using System.IO;
using System.Linq;

namespace Mp3
{
    class Program
    {
        private static Logger _logger;
        static void Main(string[] args)
        {
            if (args.Length != 1)
                return;

            var directoryPath = args.ElementAt(0);
            var musicDirectory = new DirectoryInfo(directoryPath);
            var fileNames = musicDirectory.GetFileSystemInfos("*.mp3").OrderBy(x => x.CreationTime)
                .Select(x => new { x.FullName, x.Name }).ToArray();

            var percent = Convert.ToInt32(Math.Round((float)fileNames.Length / 100));
            var logFilePath = $"{directoryPath}{DateTime.Now:MM-dd-yyyy-HH-mm}.txt";

            if (File.Exists(logFilePath))
                File.Delete(logFilePath);

            var fileStream = File.Create(logFilePath);
            fileStream.Close();

            _logger = new Logger();

            using (var logFile = new StreamWriter(logFilePath))
            {
                for (int index = 0; index < fileNames.Length; index++)
                {
                    if (index % percent == 0)
                        ProgressDisplay.RenderConsoleProgress(index / percent);

                    var fileName = fileNames[index];
                    CorrectFileTags(fileName.FullName, fileName.Name, logFile);
                }
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }

        private static void CorrectFileTags(string fullFileName, string fileName, StreamWriter logFile)
        {
            var audioFile = TagLib.File.Create(fullFileName);

            var parts = fileName.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }

            audioFile.Tag.Album = "Неизвестный альбом";
            switch (parts.Length)
            {
                case 1:
                    audioFile.Tag.Title = parts[0];
                    _logger.Log(logFile, fileName, parts[0], null, "One part");
                    break;
                case 2:
                    audioFile.Tag.AlbumArtists = new[] { parts[0] };
                    audioFile.Tag.Performers = new[] { parts[0] };
                    audioFile.Tag.Title = parts[1];
                    _logger.Log(logFile, fileName, parts[0], parts[1], "Success");
                    break;
                case 3:
                    audioFile.Tag.AlbumArtists = new[] { parts[0] };
                    audioFile.Tag.Performers = new[] { parts[0] };
                    audioFile.Tag.Title = string.Format(parts[1], "+", parts[2]);
                    _logger.Log(logFile, fileName, parts[0], parts[1], "Combined last two");
                    break;
                default:
                    _logger.Log(logFile, fullFileName, parts[0], parts[1], "Skipped");
                    break;
            }

            try
            {
                audioFile.Save();
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.Log(logFile, exception.Message);
            }
        }
    }
}
