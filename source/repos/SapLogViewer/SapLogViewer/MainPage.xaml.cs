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
        }

        private void Grid_Loading(FrameworkElement sender, object args)
        {
            ctrlDataGrid.Sorting += DataGrid_Sorting;
            ctrlDataGrid.LoadingRowGroup += DataGrid_LoadingRowGroup;
        }

        private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {

        }

        private void DataGrid_LoadingRowGroup(object sender, DataGridRowGroupHeaderEventArgs e)
        {
            //ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
            //DataGridDataItem item = group.GroupItems[0] as DataGridDataItem;
            //e.RowGroupHeader.PropertyValue = item.Range;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".zip");
            picker.FileTypeFilter.Add("*");

            IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
            if (files.Count > 0) {
                ctrlDataGrid.ItemsSource = await viewModel.GetDataAsync(files);
            }
        }
    }
}
