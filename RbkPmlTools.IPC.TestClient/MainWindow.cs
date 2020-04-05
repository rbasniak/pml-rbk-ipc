using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RbkPmlTools.IPC;
using System.Windows.Forms;
using System.Diagnostics;

namespace RbkPmlTools.IPC.TestClient
{
    public partial class MainWindow : Form
    {
        private readonly IIpcServer _receiver;
        private readonly IIpcClient _transmitter;

        private volatile string _output;

        public MainWindow()
        {
            InitializeComponent();

            _receiver = new CopyDataServer("CLIENT_RECEIVER");
            _receiver.Start();
            _receiver.Received += Receiver_Received;

            _transmitter = new CopyDataClient();
        }

        private void Receiver_Received(object sender, DataReceivedEventArgs e)
        {
            this.UIThread(() => Output.Text = e.Data);
        }

        private void Send_Click(object sender, EventArgs e)
        {
            _transmitter.Send(Input.Text, "SERVER_RECEIVER");
        }
    }

    public static class ControlExtensions
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        public static void UIThread(this Control @this, Action code)
        {
            if (@this.InvokeRequired)
            {
                @this.BeginInvoke(code);
            }
            else
            {
                code.Invoke();
            }
        }
    }
}

