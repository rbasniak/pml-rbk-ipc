using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aveva.Core.PMLNet;
using Newtonsoft.Json;
using RbkPmlTools.IPC;
using RbkPmlTools.Commands;
using System.Windows.Threading;

namespace RbkPmlTools.IPC.PMLNet
{
    [PMLNetCallable]
    public partial class RbkIpcServerControl : UserControl
    {
        private readonly IIpcServer _receiver;
        private readonly IIpcClient _transmitter;
        private readonly Dispatcher _avevaDispatcher;

        [PMLNetCallable]
        public RbkIpcServerControl()
        {
            InitializeComponent();

            _avevaDispatcher = Dispatcher.CurrentDispatcher;
            _receiver = new CopyDataServer("SERVER_RECEIVER");
            _receiver.Received += Server_Received;

            _transmitter = new CopyDataClient();
        }

        private void Server_Received(object sender, DataReceivedEventArgs e)
        {
            var cli = new CommandLineWrapper();

            _avevaDispatcher.BeginInvoke(new Action(() => 
            {
                var response = cli.Execute(new CommandLineWrapper.CommandInput(e.Data));

                _transmitter.Send(JsonConvert.SerializeObject(response, Formatting.Indented), "CLIENT_RECEIVER");
            }));
        }

        [PMLNetCallable]
        public void Assign(RbkIpcServerControl that)
        {

        }

        private void PmlInterComm_Load(object sender, EventArgs e)
        {
            try
            {
                _receiver.Start();

                StatusLabel.Text = "Status: Service is running and waiting for commands...\n\nYou can close this window now.";
            }
            catch (Exception ex)
            {
                StatusLabel.Text = "Status: Error while starting the service: " + ex.Message;
            }
        } 

        protected override void OnLeave(EventArgs e)
        {
            _receiver.Stop();

            base.OnLeave(e);
        }

        private void Send_Click(object sender, EventArgs e)
        {

        }
    }
}
