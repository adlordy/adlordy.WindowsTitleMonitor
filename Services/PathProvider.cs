using adlordy.WindowTitleMonitor.Contracts;
using System;
using System.IO;

namespace adlordy.WindowTitleMonitor.Services
{
    public class PathProvider : IPathProvider
    {
        public string GetPath(DateTime date)
        {
            return Path.Combine(GetFolderPath(), $"{date:yyyy-MM-dd}.txt");
        }

        public string GetFolderPath()
        {
            string path = Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%"), "WindowTitleMonitor");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}
