using System.Threading;

namespace adlordy.WindowTitleMonitor.Options
{
    public class ApplicationOptions
    {
        public CancellationToken ApplicationLifetime { get; set; }
        public int Delay { get; set; }
    }
}
