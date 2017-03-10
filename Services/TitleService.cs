using adlordy.WindowTitleMonitor.Contracts;
using adlordy.WindowTitleMonitor.Native;
using System;
using System.Text;

namespace adlordy.WindowTitleMonitor.Services
{
    public class TitleService : ITitleService
    {
        public string GetWindowTitle()
        {
            IntPtr foregroundWindow = WindowsShell.GetForegroundWindow();
            if (!(foregroundWindow != IntPtr.Zero))
                return null;
            StringBuilder lpString = new StringBuilder(byte.MaxValue);
            WindowsShell.GetWindowText(foregroundWindow, lpString, byte.MaxValue);
            return lpString.ToString();
        }
    }
}
