using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWW.Utility
{
    public class UnixEpochTime
    {
        public const long EPOCH = 621355968000000000;
        public static long now()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - EPOCH) / 10000;
        }
        public static long toUnixEpochTime(long time)
        {
            return (time - EPOCH) / 10000;
        }
    }
}
