using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FMWW.Http
{
    public abstract class AbstractUploader : FMWW.Core.ImportablePage, Patterns.Interfaces.ICommand
    {
        public AbstractUploader() : base() { }
        public AbstractUploader(FMWW.Http.Client client) : base(client) { }

        public string ResultMessage { get; protected set; }

        public virtual bool CanExecute(object parameter)
        {
            return !String.IsNullOrWhiteSpace(PathShiftJis)
                && (new System.IO.FileInfo(PathShiftJis).Length > 0);
        }

        public virtual void Execute(object parameter)
        {
            SignIn();
            byte[] resData = _Client.UploadValues(FMWW.Core.MainMenu.Url, SelectMainMenu().Translate());
            var _html = Encoding.GetEncoding("Shift_JIS").GetString(resData);
            ResultMessage = Import();
        }

        public event EventHandler CanExecuteChanged;

        protected virtual FMWW.Core.MainMenu SelectMainMenu()
        {
            throw new NotImplementedException();
        }
    }
}
