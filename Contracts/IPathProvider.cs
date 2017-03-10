using System;

namespace adlordy.WindowTitleMonitor.Contracts
{
    public interface IPathProvider
    {
        string GetFolderPath();
        string GetPath(DateTime date);
    }
}