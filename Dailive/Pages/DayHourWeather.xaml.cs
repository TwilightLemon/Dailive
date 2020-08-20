using System;
using System.Collections.Generic;
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
    /// DayHourWeather.xaml 的交互逻辑
    /// </summary>
    public partial class DayHourWeather : UserControl
    {
        public DayHourWeather()
        {
            InitializeComponent();
        }
        public DayHourWeather(HourWeather data)
        {
            InitializeComponent();
            time.Text = DateTime.Parse(data.Date).ToString("HH:mm");
            wind.Text ="风力 "+ data.wind+"级";
            UV.Text = "";
            desc.Text = data.Tmp + "°";
            img.Background = new ImageBrush(new BitmapImage(new Uri(GetImgUrl(data.code))));
        }
        public DayHourWeather(DailyWeather data)
        {
            InitializeComponent();
            time.Text = DateTime.Parse(data.Date).ToString("MM.dd");
            wind.Text = data.wind+"  ";
            UV.Text ="UV "+ data.uv;
            desc.Text = data.Tmp + "°";
            img.Background = new ImageBrush(new BitmapImage(new Uri(GetImgUrl(data.code))));
        }
    }
}
