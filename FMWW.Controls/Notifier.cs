using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace FMWW.Controls
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisibleAttribute(true)]
    public class Notifier
    {
        public Action<string> OnCompleted;
        public Action<string> OnTerminated;

        public void Completed(string message)
        {
            this.OnCompleted(message);
        }

        public void Terminated(string v)
        {
            if (null == this.OnTerminated)
            {
                return;
            }
            this.OnTerminated(v);
        }

        public Notifier(Action<string> onCompleted)
        {
            this.OnCompleted = onCompleted;
        }
    }
}
