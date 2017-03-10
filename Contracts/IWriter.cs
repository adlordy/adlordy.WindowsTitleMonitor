using System;

namespace adlordy.WindowTitleMonitor.Contracts
{
    public interface IWriter : IDisposable
    {
        void Write(DateTime dateTime, string title);
    }
}