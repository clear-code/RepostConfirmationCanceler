using System;
using System.IO;

namespace RepostConfirmationCanceler
{
    internal class Logger
    {
        private static readonly int MaxGeneration = 10;

        private static readonly long MaxLogSize = 10 * 1024 * 1024;

        private static readonly object LockObject = new object();

        private StreamWriter LogStream { get; set; }

        private string FilePath { get; set; }

        private string LogFileNameBase { get; }

        internal void Log(string message) => NoException(() => LogImpl(message));
        internal void Log(Exception e) => NoException(() => LogImpl(e));

        internal Logger(RunTimeMode mode)
        {
            LogFileNameBase = mode == RunTimeMode.Server ? "RepostConfirmationCanceler_server" : "RepostConfirmationCanceler_client";
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var logDirectory = Path.Combine(appDataPath, "RepostConfirmationCanceler");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            FilePath = Path.Combine(logDirectory, $"{LogFileNameBase}.log");
        }

        private void NoException(Action func)
        {
            try { func(); } catch { }
        }

        private void LogImpl(string message)
        {
            lock (LockObject)
            {
                RotateIfNeed();
                LogStream.WriteLine($"{GetTimestamp()} : {message}");
                LogStream.Flush();
            }
        }

        private void LogImpl(Exception e)
        {
            LogImpl(e.ToString());
        }

        private string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void RotateIfNeed()
        {
            if (!File.Exists(FilePath))
            {
                LogStream?.Close();
                LogStream = null;
            }

            if (LogStream is null)
            {
                var fileStream = new FileStream(FilePath, FileMode.OpenOrCreate);
                fileStream.Seek(0, SeekOrigin.End);
                LogStream = new StreamWriter(fileStream);
            }

            var fi = new FileInfo(FilePath);
            if (fi.Length > MaxLogSize)
            {
                Rotate();
            }
        }

        private void Rotate()
        {
            lock (LockObject)
            {
                LogStream?.Close();

                string previousFileName;
                string previousFilePath;
                string rotatedFileName;
                string rotatedFilePath;
                string userDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                for (int i = MaxGeneration - 1; i >= 0; i--)
                {
                    if (i > 0)
                    {
                        previousFileName = $"{LogFileNameBase}_{i}.log";
                    }
                    else
                    {
                        previousFileName = $"{LogFileNameBase}.log";
                    }

                    previousFilePath = Path.Combine(userDir, previousFileName);

                    if (!File.Exists(previousFilePath))
                    {
                        continue;
                    }
                    rotatedFileName = $"{LogFileNameBase}_{i + 1}.log";
                    rotatedFilePath = Path.Combine(userDir, rotatedFileName);

                    File.Copy(previousFilePath, rotatedFilePath, true);
                    File.Delete(previousFilePath);
                }

                LogStream = new StreamWriter(new FileStream(FilePath, FileMode.Create));
            }
        }
    }
}
