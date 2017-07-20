using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FMWW.Master.Component
{
    class ExportSupplierCompletedEventArgs : AsyncCompletedEventArgs
    {
        internal ExportSupplierCompletedEventArgs(Exception error, bool cancelled, object userState)
            : base(error, cancelled, userState)
        {
        }

        public Dictionary<string, string> Info { get; internal set; }
    }
}
