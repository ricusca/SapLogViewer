using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Threading;
using Windows.Storage.Streams;
using System.ComponentModel;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SapLogViewer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DataGridDataSource viewModel = new DataGridDataSource();
        public MainPage()
        {
            this.InitializeComponent();
            ctrlNoUsers.Text = "";
            ctrlTotalTransactions.Text = "";
            ctrlTopPUser.Text = "";
            ctrlTopTUser.Text = "";
        }

        private void Grid_Loading(FrameworkElement control, object args)
        {
            ctrlDataGrid.Sorting += DataGrid_Sorting;
            ctrlDataGrid.LoadingRowGroup += DataGrid_LoadingRowGroup;
            ctrlComboBox.ItemsSource = Enum.GetValues(typeof(DataGridGridLinesVisibility)).Cast<DataGridGridLinesVisibility>();
            ctrlComboBox.SelectedIndex = 0;
        }

        private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            string previousSortedColumn = viewModel.CachedSortedColumn;
            if (previousSortedColumn != string.Empty)
            {
                foreach (DataGridColumn dataGridColumn in ctrlDataGrid.Columns)
                {
                    if (dataGridColumn.Tag != null && dataGridColumn.Tag.ToString() == previousSortedColumn &&
                        (e.Column.Tag == null || previousSortedColumn != e.Column.Tag.ToString()))
                    {
                        dataGridColumn.SortDirection = null;
                    }
                }
            }

            // Toggle clicked column's sorting method
            if (e.Column.Tag != null)
            {
                if (e.Column.SortDirection == null)
                {
                    ctrlDataGrid.ItemsSource = viewModel.SortData(e.Column.Tag.ToString(), true);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    ctrlDataGrid.ItemsSource = viewModel.SortData(e.Column.Tag.ToString(), false);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    ctrlDataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.All,ctrlSearch.Text);
                    e.Column.SortDirection = null;
                }
            }
        }

        private void DataGrid_LoadingRowGroup(object sender, DataGridRowGroupHeaderEventArgs e)
        {
            //ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
            //DataGridDataItem item = group.GroupItems[0] as DataGridDataItem;
            //e.RowGroupHeader.PropertyValue = item.Range;
        }

        private async void Button_Load(object sender, RoutedEventArgs e)
        {
            ctrlListBox.Items.Clear();
            viewModel.Clear();
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".zip");
            picker.FileTypeFilter.Add("*");

            IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
            List<IRandomAccessStreamWithContentType> randomStreams = new List<IRandomAccessStreamWithContentType>();
            for (int i = 0; i < files.Count; i++)
            {
                ctrlListBox.Items.Add(files[i].Path);
                randomStreams.Add(await files[i].OpenReadAsync());
            }
            var bw = new BackgroundWorker();
            bw.DoWork += (s, args) =>
            {
                viewModel.ThreadPoolCallback(randomStreams);
            };

            bw.RunWorkerCompleted += (s, args) =>
            {
                ctrlDataGrid.ItemsSource = null;
                ctrlDataGrid.ItemsSource = viewModel.GetData();
                ctrlLoading.IsLoading = false;
                UpdateStatisticsUI();
            };

            bw.RunWorkerAsync();

            if (LoadingContentControl != null)
            {
                LoadingContentControl.ContentTemplate = Resources["WaitListTemplate"] as DataTemplate;
                ctrlLoading.IsLoading = true;                
            }
        }

        private void UpdateStatisticsUI()
        {
            ctrlNoUsers.Text = string.Format("No. users: {0}", Statistics.TotalUsers);
            ctrlTotalTransactions.Text = string.Format("Total Transactions: {0}", Statistics.TotalTransactions);
            ctrlTopPUser.Text = string.Format("Top Programs User: {0}", Statistics.TopPUser);
            ctrlTopTUser.Text = string.Format("Top Transactions User: {0}", Statistics.TopTUser);
        }

        private void CtrlComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ctrlComboBox.SelectedValue.ToString()))
            {
                DataGridGridLinesVisibility val;
                Enum.TryParse(ctrlComboBox.SelectedValue.ToString(), out val);
                ctrlDataGrid.GridLinesVisibility = val;
            }
        }

        private void CtrlSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(ctrlSearch.Text))
                ctrlDataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.All, ctrlSearch.Text.ToUpper());
            else
                ctrlDataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.Name, ctrlSearch.Text.ToUpper());
        }

        private void CtrlDataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            StackPanel panel = e.DetailsElement as StackPanel;
            Grid grid = panel.Children.ElementAt(0) as Grid;

            DataGrid innerGrid = panel.FindName("ctrUserRow") as DataGrid;
            //innerGrid.ItemsSource = Globals._usersData["IBAILA"].Actions;
        }
    }
}
