﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
namespace SapLogViewer
{
    [Bindable]
    public class DataGridDataSource
    {
        private static ObservableCollection<User> _items = new ObservableCollection<User>();
//        private static CollectionViewSource groupedItems;
        private string _cachedSortedColumn = string.Empty;

        public void Clear()
        {
            _items.Clear();
            Globals._usersData.Clear();
            Globals.Clear();
        }

        public IEnumerable<User> GetData()
        {
            foreach(var user in Globals._usersData.Values)
            {
                _items.Add(user);
            }
            return _items;
        }

        public string CachedSortedColumn
        {
            get
            {
                return _cachedSortedColumn;
            }

            set
            {
                _cachedSortedColumn = value;
            }
        }

        public enum FilterOptions
        {
            All = -1,
            Name = 0,
            Program = 1,
            Transaction = 2,
            PC = 3,
        }

        public ObservableCollection<User> SortData(string sortBy, bool ascending)
        {
            _cachedSortedColumn = sortBy;
            switch (sortBy)
            {
                case "Name":
                    if (ascending)
                    {
                        return new ObservableCollection<User>(from item in _items
                                                                          orderby item.Name ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<User>(from item in _items
                                                                          orderby item.Name descending
                                                                          select item);
                    }

                case "TotalPrograms":
                    if (ascending)
                    {
                        return new ObservableCollection<User>(from item in _items
                                                                          orderby item.TotalPrograms ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<User>(from item in _items
                                                                          orderby item.TotalPrograms descending
                                                                          select item);
                    }

                case "TotalTransactions":
                    if (ascending)
                    {
                        return new ObservableCollection<User>(from item in _items
                                                                          orderby item.TotalTransactions ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<User>(from item in _items
                                                                          orderby item.TotalTransactions descending
                                                                          select item);
                    }
            }

            return _items;
        }

        public ObservableCollection<User> FilterData(FilterOptions filterBy, string val)
        {
            switch (filterBy)
            {
                case FilterOptions.All:
                    return new ObservableCollection<User>(_items);

                case FilterOptions.Name:
                    return new ObservableCollection<User>(from item in _items
                                                          where item.Name.ToUpper().Contains(val)
                                                          select item);

                case FilterOptions.Transaction:
                    return new ObservableCollection<User>(from item in _items
                                                          where item.GetPrograms().Where(x => x.ToUpper().Contains(val)).Count() > 0
                                                          select item);

                case FilterOptions.Program:
                    return new ObservableCollection<User>(from item in _items
                                                          where item.GetPrograms().Where(x => x.ToUpper().Contains(val)).Count() > 0
                                                          select item);
            }

            return _items;
        }

        public void ThreadPoolCallback(object state)
        {
            List<IRandomAccessStreamWithContentType> streams = state as List<IRandomAccessStreamWithContentType>;
            List<DataGridDataItem> logItems = new List<DataGridDataItem>();
            Parallel.ForEach(streams, (randomStream) =>
            {
                List<DataGridDataItem> items = new List<DataGridDataItem>();
                using (StreamReader sr = new StreamReader(randomStream.AsStreamForRead()))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] values = line.Split('|');
                        if (values.Length > 9 && values[1].ToCharArray()[0] != 'D')
                        {

                            items.Add(
                                new DataGridDataItem()
                                {
                                    Date = Globals.GetStringId(ref Globals._dates, values[1]).ToString(),
                                    Time = values[2],
                                    Cl = values[3],
                                    User = Globals.GetStringId(ref Globals._users,values[4]).ToString(),
                                    PCName      = Globals.GetStringId(ref Globals._pc, values[5]).ToString(),
                                    Transaction = Globals.GetStringId(ref Globals._transactions, values[6]).ToString(),
                                    Program     = Globals.GetStringId(ref Globals._programs, values[7]).ToString(),
                                    //AuditLog    = Globals.GetStringId(ref Globals._auditLog, values[8]).ToString(),
                                    //VarLog      = Globals.GetStringId(ref Globals._varLog, values[9]).ToString()
                                });

                        }
                    }
                }
                lock (logItems)
                {
                    items.ForEach(logItems.Add);
                }
                items.Clear();
                System.GC.Collect();
            });

            int Id = 1;
            foreach(var item in logItems)
            {
                if (!Globals._usersData.ContainsKey(item.User))
                {
                    var user = new User(new List<DataGridDataItem>(), Id++, item.User);
                    Globals._usersData.Add(user.Name, user);
                }
                Globals._usersData[item.User].Actions.Add(item);
            }

            Statistics.Update(ref Globals._usersData);

            logItems.Clear();           
        }
    }
}
