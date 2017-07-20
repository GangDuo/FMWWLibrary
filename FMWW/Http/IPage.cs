using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Http
{
    public interface IPage
    {
        event Action<IPage> GoneAway;
        event Action<string> CsvDownloadCompleted;
        event Action<byte[]> ExcelDownloadCompleted;

        void Quit();
        void Register();
        void Search();
        void Del();
        void Print();
        void CsvAsync();
        string Csv();
        void ExcelAsync();
        byte[] Excel();
        void Preview();
        void Check();
        void Cancel();
        void Xqt();
    }
}
