using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using Xml2CSharp;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage2 : Page
    {

        public NewPage2()
        {
            this.InitializeComponent();
        }

        private async void SearchWeatherButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            string input = SearchInputWeather.Text;
            RootObject data = await OpenGeoProxy.GetGeoInformation(input);
            string result = "| : Cityname: " + input + "\n| : "
                            + "Humidity: " + data.result.realtime.weather.humidity + "\n| : "
                            + "Img: " + data.result.realtime.weather.img + "\n| : "
                            + "Info: " + data.result.realtime.weather.info + "\n| : "
                            + "Temperature: " + data.result.realtime.weather.temperature;
            Weather.Text = result;
        }
        private async void SearchAddressButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            string input = SearchInputAddress.Text;
            string input2 = SearchInputAddress2.Text;
            CoordAddressResult data = await xmlway.GetWeather(input, input2);
            string result = "| : Cityname: " + input + "\n| : "
                            + "Country: " + data.Result.Country + "\n| : "
                            + "Country_code: " + data.Result.Country_code + "\n| : "
                            + "Province: " + data.Result.Province + "\n| : "
                            + "City: " + data.Result.City + "\n| : "
                            + "District: " + data.Result.District + "\n| : "
                            + "Street: " + data.Result.Street + "\n| : "
                            + "Street_number: " + data.Result.Street_number;
            Address.Text = result;
        }

        private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }

}


