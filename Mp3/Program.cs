using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagLib.Riff;

namespace Mp3
{
    class Program
    {
        static void Main(string[] args)
        {
            var folderPath = @"C:\Users\Lisagi\Music\test\";
            var musicDirectory = new DirectoryInfo(folderPath);
            var fileNames = musicDirectory.GetFileSystemInfos("*.mp3").OrderByDescending(x => x.CreationTime)
                .Select(x => new { FullName = x.FullName, Name = x.Name }).ToArray();

            var logPath = string.Format("C:\\Users\\Lisagi\\Music\\test\\{0}.txt", DateTime.Now.ToString(@"MM-dd-yyyy-HH-mm"));

            if (System.IO.File.Exists(logPath))
                System.IO.File.Delete(logPath);

            var fileStream = System.IO.File.Create(logPath);
            fileStream.Close();

            using (StreamWriter logFile = new StreamWriter(logPath))
            {
                foreach (var fileName in fileNames)
                    CorrectFileTags(fileName.FullName, fileName.Name, logFile);
            }

            Console.WriteLine("completed");
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
            if (parts.Length == 1)
            {
                audioFile.Tag.Title = parts[0];
                Log(logFile, fileName, parts[0], null, "One part");
            }
            else if (parts.Length == 2)
            {
                audioFile.Tag.AlbumArtists = new[] { parts[0] };
                audioFile.Tag.Performers = new[] { parts[0] };
                audioFile.Tag.Title = parts[1];
                Log(logFile, fileName, parts[0], parts[1], "Success");
            }
            else if (parts.Length == 3)
            {
                audioFile.Tag.AlbumArtists = new[] { parts[0] };
                audioFile.Tag.Performers = new[] { parts[0] };
                audioFile.Tag.Title = string.Format(parts[1], "+", parts[2]);
                Log(logFile, fileName, parts[0], parts[1], "Combined last two");
            }
            else
            {
                Log(logFile, fullFileName, parts[0], parts[1], "Skip");
            }

            audioFile.Save();
        }

        private static void Log(StreamWriter file, string fileName, string title, string artist, string status)
        {
            var logRecord = string.Format("status: {0}, fileName: {1}, title: {2}, artist: {3}", status, fileName, title, artist);

            try
            {
                file.WriteLine(logRecord);
             //   file.Flush();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
