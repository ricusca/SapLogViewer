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
        public String Date { get; set; }
        public String Time { get; set; }
        public String Cl { get; set; }
        public String User { get; set; }
        public String PCName { get; set; }
        public String Transaction { get; set; }
        public String Program { get; set; }
        public String AuditLog { get; set; }
        public String VarLog { get; set; }

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
