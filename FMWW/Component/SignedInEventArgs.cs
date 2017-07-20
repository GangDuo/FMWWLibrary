using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FMWW.Component
{
    public class SignedInEventArgs : AsyncCompletedEventArgs
    {
        internal SignedInEventArgs(Exception error, bool cancelled, object userState)
            : base(error, cancelled, userState)
        {
        }


        // 概要:
        //     Overload:System.Net.WebClient.DownloadDataAsync メソッドによってダウンロードされたデータを取得します。
        //
        // 戻り値:
        //     ダウンロードされたデータが格納されている System.Byte 配列。
        public byte[] Result { get; internal set; }
    }
}
