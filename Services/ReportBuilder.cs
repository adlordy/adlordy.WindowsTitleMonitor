using adlordy.WindowTitleMonitor.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace adlordy.WindowTitleMonitor.Services
{
    public class ReportBuilder : IReportBuilder
    {
        private readonly IPathProvider _provider;

        public ReportBuilder(IPathProvider provider)
        {
            _provider = provider;
        }

        public void Build(DateTime date)
        {
            string path = _provider.GetPath(date);

            var items = from line in GetLineEnumerator(new StreamReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Write)))
                        let value = Split(line)
                        where value != null
                        group value by value into g
                        select g;

            using (StreamWriter text = File.CreateText(Path.Combine(_provider.GetFolderPath(), $"{date:yyyy-MM-dd}.csv")))
            {
                text.WriteLine("Title\tCount");
                foreach (var item in items)
                    text.WriteLine($"{item.Key}\t{item.Count()}");
                text.Flush();
            }
        }

        private IEnumerable<string> GetLineEnumerator(StreamReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }

        private string Split(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            int num = value.IndexOf('\t');
            if (num < 0)
                return null;
            return value.Substring(num + 1);
        }
    }
}
