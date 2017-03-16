using System;
using System.Runtime.InteropServices;
using System.Text;

namespace adlordy.WindowTitleMonitor.Native
{
  public class WindowsShell
  {
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern int GetWindowTextLength(IntPtr hWnd);
  }
}
