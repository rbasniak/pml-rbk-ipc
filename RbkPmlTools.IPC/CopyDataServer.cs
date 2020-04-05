using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RbkPmlTools.IPC
{
    public class CopyDataServer : IIpcServer
    {
        private string _name;
        private NativeWindow _messageHandler;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, IntPtr wparam, IntPtr lparam);

        private const int WM_COPY_DATA = 0x004A;
        private const int WM_QUIT = 0x0012;

        public CopyDataServer(string name = null)
        {
            _name = name;
        }

        public string Name => _name ?? nameof(IIpcServer);

        private sealed class MessageHandler : NativeWindow
        {
            private readonly CopyDataServer _server;

            public MessageHandler(CopyDataServer server)
            {
                _server = server;
                CreateHandle(new CreateParams() { Caption = server.Name });
            }

            protected override void WndProc(ref Message msg)
            {
                if (msg.Msg == WM_COPY_DATA)
                {
                    var cds = (COPYDATASTRUCT)Marshal.PtrToStructure(msg.LParam, typeof(COPYDATASTRUCT));

                    if (cds.cbData.ToInt32() > 0)
                    {
                        var bytes = new byte[cds.cbData.ToInt32()];

                        Marshal.Copy(cds.lpData, bytes, 0, cds.cbData.ToInt32());

                        var chars = Encoding.ASCII.GetChars(bytes);
                        var data = new string(chars);

                        _server.OnReceived(new DataReceivedEventArgs(data));
                    }

                    msg.Result = (IntPtr)1;
                }

                base.WndProc(ref msg);
            }
        }

        private void OnReceived(DataReceivedEventArgs e)
        {
            Received?.Invoke(this, e);
        }

        void IDisposable.Dispose()
        {
            Stop();
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                _messageHandler = new MessageHandler(this);

                Application.Run();
            });
        }

        public void Stop()
        {
            SendMessage(_messageHandler.Handle, WM_QUIT, IntPtr.Zero, IntPtr.Zero);
        }

        public event EventHandler<DataReceivedEventArgs> Received;
    }
}
