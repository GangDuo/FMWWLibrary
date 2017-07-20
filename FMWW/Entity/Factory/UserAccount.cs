using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FMWW.Entity.Factory
{
    public class UserAccount
    {
        public static Entity.UserAccount Load(string path)
        {
            return Load(path, Encoding.GetEncoding("Shift_JIS"));
        }

        public static Entity.UserAccount Load(string path, Encoding encoding)
        {
            if (".json" != Path.GetExtension(path).ToLower())
            {
                throw new Exception("拡張子が正しくありません。");
            }

            using (var sr = new StreamReader(path, encoding))
            {
                var text = sr.ReadToEnd();
                if (text.Length == 0)
                {
                    throw new Exception("アカウント未設定");
                }
                else
                {
                    return Text.Json.Parse<Entity.UserAccount>(text);
                }
            }
        }
    }
}
