using adlordy.WindowTitleMonitor.Contracts;
using System;
using System.Globalization;
using System.IO;

namespace adlordy.WindowTitleMonitor.Services
{
    public class FileWriter : IWriter
    {
        private readonly IPathProvider _provider;
        private StreamWriter _writer;

        private DateTime _lastDatetime;

        public FileWriter(IPathProvider provider)
        {
            _provider = provider;
        }

        public void Dispose()
        {
            DisposeWriter();
        }

        public void Write(DateTime dateTime, string title)
        {
            if (dateTime.Date != _lastDatetime.Date)
            {
                DisposeWriter();
                CreateWriter(dateTime);
            }
            _lastDatetime = dateTime;
            _writer.Write($"{dateTime.ToString("s", CultureInfo.InvariantCulture)}\t{title}{Environment.NewLine}");
        }

        private void CreateWriter(DateTime dateTime)
        {
            _writer = new StreamWriter(File.Open(_provider.GetPath(dateTime),
                FileMode.Append, FileAccess.Write, FileShare.Read));
        }

        private void DisposeWriter()
        {
            _writer?.Flush();
            _writer?.Dispose();
        }
    }
}
