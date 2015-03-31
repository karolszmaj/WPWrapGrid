using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using InstagramWrapper.Service;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using WrapGrid.Example.Resources;
using WrapGrid.Example.Services.Client;

namespace WrapGrid.Example
{
    public partial class MainPage : PhoneApplicationPage
    {
        private SplashBaseClient client;
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;
            client = new SplashBaseClient();
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var result = await client.GetImagesAsync(0, 200);
            var @string = JsonConvert.SerializeObject(result);
            DataGrid.ItemsSource = result;
        }


    }
}