using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Profile.Admin
{
    public enum Users
    {
        Admin = 0,
        Guest
    }

    static public class UserAccount
    {
        public static readonly string UserName = Environment.GetEnvironmentVariable("FMWW_ORGANIZATION_CODE", EnvironmentVariableTarget.User);
        public static readonly string PassWord = Environment.GetEnvironmentVariable("FMWW_ORGANIZATION_PASS", EnvironmentVariableTarget.User);
        public static readonly string Person = Environment.GetEnvironmentVariable("FMWW_USER_CODE", EnvironmentVariableTarget.User);
        public static readonly string PersonPassword = Environment.GetEnvironmentVariable("FMWW_USER_PASS", EnvironmentVariableTarget.User);

        private static readonly string[] ClientQueryNames = new string[] { "form1:client", "form1:username" };
        private static readonly string[] PersonQueryNames = new string[] { "form1:person_code", "form1:person" };
        private static readonly string[] PasswordQueryNames = new string[] { "form1:password", "form1:clpass" };
        private static readonly string[] PersonPasswordQueryNames = new string[] { "form1:person_password", "form1:pspass" };

        private static Dictionary<HashSet<string>, string> _conf = new Dictionary<HashSet<string>, string>()
            {
                {new HashSet<string>(ClientQueryNames), UserName},
                {new HashSet<string>(PersonQueryNames), Person},
                {new HashSet<string>(PasswordQueryNames), PassWord},
                {new HashSet<string>(PersonPasswordQueryNames), PersonPassword},
            };

        public static Dictionary<HashSet<string>, string> Conf
        {
            get { return _conf; }
        }

        public static bool HasEntry(string entry)
        {
            try
            {
                EntryValue(entry);
                return true;
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public static string EntryValue(string entry)
        {
            foreach (HashSet<string> entries in UserAccount.Conf.Keys)
            {
                if (entries.Contains(entry))
                {
                    return UserAccount.Conf[entries];
                }
            }
            throw new System.Collections.Generic.KeyNotFoundException();
        }

        /**
         foo://example.com:8042/over/there?name=ferret#nose
         \_/   \______________/\_________/ \_________/ \__/
          |           |            |            |        |
       scheme     authority       path        query   fragment
        */
        public static bool IsClientQueryName(string name)
        {
            return ClientQueryNames.Contains(name);
        }
        public static bool IsPersonQueryName(string name)
        {
            return PersonQueryNames.Contains(name);
        }
        public static bool IsPasswordQueryName(string name)
        {
            return PasswordQueryNames.Contains(name);
        }
        public static bool IsPersonPasswordQueryName(string name)
        {
            return PersonPasswordQueryNames.Contains(name);
        }
    }
}
