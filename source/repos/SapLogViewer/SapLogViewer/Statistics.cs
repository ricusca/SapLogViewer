using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapLogViewer
{
    public class Statistics
    {
        public static Int64 TotalTransactions { get; set; }
        public static Int64 TotalPrograms { get; set; }
        public static Int64 TotalUsers { get; set; }
        public static String TopTUser { get; set; }
        public static String TopPUser { get; set; }

        public static void Update(ref Dictionary<string, User> usersData)
        {
            TotalUsers = usersData.Count;
            TotalTransactions = usersData.Values.Sum(u => u.TotalTransactions);
            TotalPrograms = usersData.Values.Sum(u => u.TotalPrograms);
            TopPUser = usersData.Values.OrderByDescending(u => TopPUser).First().Name;
            TopTUser = usersData.Values.OrderByDescending(u => TopTUser).First().Name;
        }

    }
}
