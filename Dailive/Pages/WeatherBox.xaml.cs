using Dailive.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Dailive.Helpers.WeatherApi;

namespace Dailive.Pages
{
    /// <summary>
    /// WeatherBox.xaml 的交互逻辑
    /// </summary>
    public partial class WeatherBox : UserControl
    {
        public WeatherBox()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                City c = Settings.USettings.Weather_Place == "ip"
                    ? await GetPositionByIpAsync()
                    : (await SearchCitiesByNameAsync(Settings.USettings.Weather_Place))[0];
                Weather data = await GetWeatherAsync(c);
                LoadWeatherData(c, data);
            }
        }

        private void LoadWeatherData(City c,Weather data) {
            CityName.Text = c.name;
            UpdateTime.Text = DateTime.Now.ToString();
            Now_Tmp.Text = data.tmp + "°";
            Now_txt.Text = data.txt;
            Now_AQI.Text = "AQI " + data.AqiValue + " " + data.AqiLevel;
            Now_UV.Text = "UV " + data.UV;
            Now_img.Background = new ImageBrush(new BitmapImage(new Uri(GetImgUrl(data.code))));
            Now_Wind.Text = data.Wind + " " + data.WindLevel + "级";
            Now_hum.Text = "相对湿度 " + data.hum + "%";
            //    string[] lw = data.desc.Split('，');
            LittleWord.Text = data.desc;// lw[0] + "\r\n" + lw[1] + "\r\n" + lw[2] + " " + lw[3].Replace("。", "");

            var tk = new Thickness(0, 0, 10, 10);
            HoursWP.Children.Clear();
            foreach (HourWeather a in data.Hours)
            {
                DayHourWeather d = new DayHourWeather(a);
                d.Margin = tk;
                HoursWP.Children.Add(d);
            }

            DailyWP.Children.Clear();
            foreach (DailyWeather a in data.Daily)
            {
                DayHourWeather d = new DayHourWeather(a);
                d.Margin = tk;
                DailyWP.Children.Add(d);
            }
        }

        private async void CityName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter&&CityName.Text!=string.Empty) {
                City c = (await SearchCitiesByNameAsync(CityName.Text))[0];
                Weather data = await GetWeatherAsync(c);
                LoadWeatherData(c, data);
                Settings.USettings.Weather_Place = CityName.Text;
            }
        }

        private async void IPlaceBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            City c = await GetPositionByIpAsync();
            Weather data = await GetWeatherAsync(c);
            LoadWeatherData(c, data);
            Settings.USettings.Weather_Place = "ip";
        }
    }
}
