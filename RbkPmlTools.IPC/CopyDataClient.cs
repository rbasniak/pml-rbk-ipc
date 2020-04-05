using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RbkPmlTools.IPC
{
    public class CopyDataClient : IIpcClient
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private const int HWND_BROADCAST = 0xffff;

        private const int WM_COPY_DATA = 0x004A;

        public void Send(string data, string serverName = nameof(IIpcServer))
        {
            var cds = new COPYDATASTRUCT();
            cds.dwData = (IntPtr)Marshal.SizeOf(cds);
            cds.cbData = (IntPtr)data.Length;
            cds.lpData = Marshal.StringToHGlobalAnsi(data);

            var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(cds));

            Marshal.StructureToPtr(cds, ptr, true);

            var target = FindWindow(null, serverName);  //(IntPtr)HWND_BROADCAST;

            var result = SendMessage(target, WM_COPY_DATA, IntPtr.Zero, ptr);

            Marshal.FreeHGlobal(cds.lpData);
            Marshal.FreeCoTaskMem(ptr);
        }
    }
}
