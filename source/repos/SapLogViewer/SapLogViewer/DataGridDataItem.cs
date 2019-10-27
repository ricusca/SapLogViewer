using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapLogViewer
{
    public class DataGridDataItem : INotifyDataErrorInfo, IComparable
    {
        private int _dateId;
        private int _userId;
        private int _pcId;
        private int _transactionId;
        private int _programId;
        private int _auditLogId;
        private int _varLogId;
        private string _programs;
        private string _transactions;
        public String Date { get { return Globals._dates[_dateId]; } set { _dateId = Int32.Parse(value); } }
        public String Time { get; set; }
        public String Cl { get; set; }
        public String User { get { return Globals._users[_userId]; }  set { _userId = Int32.Parse(value); } }
        public String PCName { get { return Globals._pc[_pcId]; } set { _pcId = Int32.Parse(value); } }
        public String Transaction { get { return Globals._transactions[_transactionId]; } set { _transactionId = Int32.Parse(value); } }
        public String Program { get { return Globals._programs[_programId]; } set { _programId = Int32.Parse(value); } }
        public String AuditLog { get { return Globals._auditLog[_auditLogId]; } set { _auditLogId = Int32.Parse(value); } }
        public String VarLog { get { return Globals._varLog[_varLogId]; } set { _varLogId = Int32.Parse(value); } }

        public String Programs
        {
            get
            {
                if (string.IsNullOrEmpty(_programs))
                {
                    var p = Globals._usersData[User].Programs();
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
                    var p = Globals._usersData[User].Transactions();
                    if (p.Count > 0)
                        _transactions = p.Count.ToString() + " -> " + string.Join(", ", p);
                    else
                        _transactions = "0";
                }
                return _transactions;
            }
        }

        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        bool INotifyDataErrorInfo.HasErrors
        {
            get
            {
                return _errors.Keys.Count > 0;
            }
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            if (propertyName == null)
            {
                propertyName = string.Empty;
            }

            if (_errors.Keys.Contains(propertyName))
            {
                return _errors[propertyName];
            }
            else
            {
                return null;
            }
        }

        int IComparable.CompareTo(object obj)
        {
            int lnCompare = Date.CompareTo((obj as DataGridDataItem).Date);

            if (lnCompare == 0)
            {
                return Time.CompareTo((obj as DataGridDataItem).Time);
            }
            else
            {
                return lnCompare;
            }
        }
    }
}
