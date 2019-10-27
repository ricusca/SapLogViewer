using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapLogViewer
{
    public class Globals
    {
        public static List<string> _dates = new List<string>();
        public static List<String> _users = new List<String>();
        public static List<String> _pc = new List<String>();
        public static List<String> _transactions = new List<string>();
        public static List<String> _programs = new List<string>();
        public static List<String> _auditLog = new List<string>();
        public static List<String> _varLog = new List<String>();

        public static Dictionary<string, User> _usersData = new Dictionary<string, User>();

        public static void Clear()
        {
            _dates.Clear();
            _users.Clear();
            _pc.Clear();
            _transactions.Clear();
            _programs.Clear();
            _auditLog.Clear();
            _varLog.Clear();
        }

        public static int GetStringId(ref List<string> cache, string data)
        {
            data = data.Trim();
            int idx = cache.IndexOf(data);
            if (idx == -1)
            {
                idx = cache.Count;
                cache.Add(data);
            }
            return idx;
        }
    }
}