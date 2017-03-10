using System;

namespace adlordy.WindowTitleMonitor.Contracts
{
    public interface IReportBuilder
    {
        void Build(DateTime date);
    }
}