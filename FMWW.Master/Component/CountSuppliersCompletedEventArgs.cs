using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FMWW.Master.Component
{
    public class CountSuppliersCompletedEventArgs : AsyncCompletedEventArgs
    {
        internal CountSuppliersCompletedEventArgs(Exception error, bool cancelled, object userState)
            : base(error, cancelled, userState)
        {
        }

        public uint Count { get; internal set; }
    }
}
