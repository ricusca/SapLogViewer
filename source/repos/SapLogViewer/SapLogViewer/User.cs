using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapLogViewer
{
    public class User
    {
        public List<DataGridDataItem> Actions { get; set; }

        public string _dateInterval;
        public string _timeInterval;
        public int _totalPrograms;
        public int _topProgram;
        public int _totalTransaction;
        public int _topTransaction;

        public int Id { get; set; }
        public string Name { get; set; }
        public string DateInterval { get; set; }
        public int TimeInterval { get; set; }
        public int TotalPrograms { get; set; }
        public int TopProgram { get; set; }
        public int TotalTransactions { get; set; }
        public int TopTransaction { get; set; }

        public User(List<DataGridDataItem> _a, int _id, string _name)
        {
            Actions = _a;
            Id = _id;
            Name = _name;
        }

        public List<String> Programs()
        {
            var r = Actions.Where(t => !string.IsNullOrEmpty(t.Program)).GroupBy(u => u.Program).Select(u => u.First().Program).ToList();
            return r;
        }

        public List<String> Transactions()
        {
            var r = Actions.Where(t => !string.IsNullOrEmpty(t.Transaction)).GroupBy(u => u.Transaction).Select(u => u.First().Transaction).ToList();
            return r;
        }
    }
}
