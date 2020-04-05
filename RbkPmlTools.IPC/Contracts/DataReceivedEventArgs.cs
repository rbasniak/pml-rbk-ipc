using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RbkPmlTools.IPC
{
    public class DataReceivedEventArgs : EventArgs
    {
        public DataReceivedEventArgs(string data)
        {
            Data = data;
        }

        public string Data { get; set; }
    }
}
