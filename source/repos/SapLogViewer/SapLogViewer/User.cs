using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapLogViewer
{
    public class User
    {
        public User()
        {
            Actions = new List<DataGridDataItem>();
        }
        public User(List<DataGridDataItem> _a, int _id, string _name)
        {
            Actions = _a;
            Id = _id;
            Name = _name;
        }

        public List<DataGridDataItem> Actions { get; set; }

        public string _dateInterval;
        public string _timeInterval;
        public int _totalPrograms;
        public string _topProgram;
        public int _totalTransactions;
        public string _topTransaction;
        private string _programs;
        private string _transactions;

        public int Id { get; set; }
        public string Name { get; set; }
        public string DateInterval { get; set; }
        public string TimeInterval { get; set; }
        public int TotalPrograms
        {
            get
            {
                if (_totalPrograms < 1)
                {
                    _totalPrograms = Actions.Where(t => !string.IsNullOrEmpty(t.Program)).GroupBy(u => u.Program).Count();
                }
                return _totalPrograms;
            }
        }
        public string TopProgram
        {
            get
            {
                if (string.IsNullOrEmpty(_topProgram))
                {
                    var p = Actions.Where(t => !string.IsNullOrEmpty(t.Program)).GroupBy(u => u.Program).OrderByDescending(c => c.Count());
                    if (p.Count() > 0)
                    {
                        _topProgram = p.First().Key;
                    }
                }
                return _topProgram;
            }
        }
        public int TotalTransactions
        {
            get
            {
                if (_totalTransactions < 1)
                {
                    _totalTransactions = Actions.Where(t => !string.IsNullOrEmpty(t.Transaction)).GroupBy(u => u.Transaction).Count();
                }
                return _totalTransactions;
            }
        }
        public string TopTransaction
        {
            get
            {
                if (string.IsNullOrEmpty(_topTransaction))
                {
                    var t = Actions.Where(x => !string.IsNullOrEmpty(x.Transaction)).GroupBy(u => u.Transaction).OrderByDescending(c => c.Count());
                    if (t.Count() > 0)
                    {
                        _topTransaction = t.First().Key;
                    }
                }
                return _topTransaction;
            }
        }



        public String Programs
        {
            get
            {
                if (string.IsNullOrEmpty(_programs))
                {
                    var p = GetPrograms();
                    if (p.Count > 0)
                        _programs = p.Count.ToString() + " -> " + string.Join(", ", p);
                    else
                        _programs = "0";
                }
                return _programs;
            }
        }

        public String Transactions
        {
            get
            {
                if (string.IsNullOrEmpty(_transactions))
                {
                    var p = GetTransactions();
                    if (p.Count > 0)
                        _transactions = p.Count.ToString() + " -> " + string.Join(", ", p);
                    else
                        _transactions = "0";
                }
                return _transactions;
            }
        }

        public List<String> GetPrograms()
        {
            var r = Actions.Where(t => !string.IsNullOrEmpty(t.Program)).GroupBy(u => u.Program).Select(u => u.First().Program).ToList();
            return r;
        }

        public List<String> GetTransactions()
        {
            var r = Actions.Where(t => !string.IsNullOrEmpty(t.Transaction)).GroupBy(u => u.Transaction).Select(u => u.First().Transaction).ToList();
            return r;
        }

    }
}
