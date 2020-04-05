using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RbkPmlTools.IPC
{
    public interface IIpcServer : IDisposable
    {
        void Start();
        void Stop();

        event EventHandler<DataReceivedEventArgs> Received;
    }
}
