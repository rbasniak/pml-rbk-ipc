using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RbkPmlTools.IPC
{
    public interface IIpcClient
    {
        void Send(string data, string serverName = nameof(IIpcServer));
    }
}
