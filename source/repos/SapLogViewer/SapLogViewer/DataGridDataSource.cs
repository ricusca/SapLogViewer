using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
namespace SapLogViewer
{
    [Bindable]
    public class DataGridDataSource
    {
        private static ObservableCollection<DataGridDataItem> _items;
        private static List<string> _users;
//        private static CollectionViewSource groupedItems;
        private string _cachedSortedColumn = string.Empty;
        private Dictionary<string, List<DataGridDataItem>> _usersData;

        private string[] formats = {"dd.MM.yyy", "hh:mm:ss"};

        public async Task<IEnumerable<DataGridDataItem>> GetDataAsync(IReadOnlyList<StorageFile> files)
        {
            List<IRandomAccessStreamWithContentType> streams = new List<IRandomAccessStreamWithContentType>();
            _usersData = new Dictionary<string, List<DataGridDataItem>>();
            _items = new ObservableCollection<DataGridDataItem>();
            List < DataGridDataItem > logItems = new List<DataGridDataItem>();
            foreach(var file in files) {
                IRandomAccessStreamWithContentType randomStream = await file.OpenReadAsync();
                streams.Add(randomStream);
            }
            
            Parallel.ForEach(streams, (randomStream) =>
            {
                List<DataGridDataItem> items = new List<DataGridDataItem>();
                using (StreamReader sr = new StreamReader(randomStream.AsStreamForRead()))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] values = line.Split('|');
                        //Regex g = new Regex(@"\|(\d{2}\.\d{2}\.\d{4})\|(\d{2}:\d{2}:\d{2})\|(\d*|)\|(.*?)\|(.*?)\|(.*?)\|(.*?)\|(.*?)\|(.*?)\|", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        //Match m = g.Match(line);
                        //if (m.Success)
                        if (values.Length > 9 && values[1].ToCharArray()[0] != 'D')
                        {
                            items.Add(
                                new DataGridDataItem()
                                {
                                    Date        = values[1],
                                    Time        = values[2],
                                    Cl          = values[3],
                                    User        = values[4],
                                    PCName      = values[5],
                                    Transaction = values[6],
                                    Program     = values[7],
                                    AuditLog    = values[8],
                                    VarLog      = values[9]
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

            foreach(var item in logItems)
            {
                if (!_usersData.ContainsKey(item.User))
                {
                    _usersData.Add(item.User, new List<DataGridDataItem>());
                    _items.Add(item);
                }
                _usersData[item.User].Add(item);
            }
            logItems.Clear();

            return _items;
        }
    }
}
