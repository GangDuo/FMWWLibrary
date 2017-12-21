using FMWW.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FMWW.Core
{
    abstract public class AbstractAuthentication
    {
        protected static readonly Encoding ShiftJIS = Encoding.GetEncoding("Shift_JIS");

        public event SignedInEventHandler SignedIn;
        protected virtual void OnSignedIn(SignedInEventArgs e)
        {
            if (SignedIn != null)
                SignedIn(this, e);
        }
    }
}
