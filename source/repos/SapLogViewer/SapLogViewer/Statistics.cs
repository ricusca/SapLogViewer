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
            TotalTransactions = 0;
            int currentMaxTransactions = -1, currentMaxPrograms = -1;
            foreach(var user in usersData)
            {
                foreach(var userAction in user.Value.Actions)
                {
                    if (!string.IsNullOrEmpty(userAction.Transaction))
                    {
                        TotalTransactions++;

                        user.Value.TotalTransactions++;
                        if (user.Value.TotalTransactions > currentMaxTransactions)
                        {
                            currentMaxTransactions = user.Value.TotalTransactions;
                            TopTUser = user.Key;
                        }
                    }

                    if (!string.IsNullOrEmpty(userAction.Program))
                    {
                        TotalPrograms++;
                        user.Value.TotalPrograms++;
                        
                        if (!string.IsNullOrEmpty(userAction.Program))
                        {
                            user.Value.TotalTransactions++;
                            if (user.Value.TotalPrograms > currentMaxPrograms)
                            {
                                currentMaxPrograms = user.Value.TotalPrograms;
                                TopPUser = user.Key;
                            }
                        }
                    }
                }
            }
        }

    }
}
