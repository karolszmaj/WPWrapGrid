using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WrapGrid.Example.Resources;
using WrapGrid.Example.Services.Client;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using WrapGrid.Example.Services.Model;

namespace WrapGrid.Example
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        private SplashBaseCachedClient client;
        private ObservableCollection<ImageModel> images;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ImageModel> Images
        {
            get { return images; }
            set { images = value; OnPropertyChanged(); }
        }


        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            client = new SplashBaseCachedClient();
            Images = new ObservableCollection<ImageModel>();
            this.Loaded += MainPage_Loaded;

        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var result = await client.GetImagesAsync(0, 5);
            Images = new ObservableCollection<ImageModel>(result);
        }

        private void ButtonRemoveClickEventHandler(object sender, RoutedEventArgs e)
        {
            Images.Remove(Images.Last());
        }

        private void ButtonForceGCEventHandler(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private async void DataGrid_FetchMoreData(object sender, System.EventArgs e)
        {
            var result = await client.GetImagesAsync(Images.Count, 10);

            foreach (var item in result)
            {
                Images.Add(item);
            }
        }


    }
}