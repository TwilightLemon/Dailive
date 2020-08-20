using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dailive.Helpers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using System.Windows.Media.Animation;

namespace Dailive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Settings.Load();
            InitializeComponent();
        }
        #region WindowBase Method
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void CloseBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.WindowState == WindowState.Normal)
                    WindowState = WindowState.Maximized;
                else WindowState = WindowState.Normal;
            }
        }

        private void MinBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        #endregion
        Storyboard ShowBackBtn;
        private void RunAnimation(DependencyObject TPage, Thickness value = new Thickness())
        {
            var sb = Resources["NSPageAnimation"] as Storyboard;
            foreach (Timeline ac in sb.Children)
            {
                Storyboard.SetTarget(ac, TPage);
                if (ac is ThicknessAnimationUsingKeyFrames)
                {
                    var ta = ac as ThicknessAnimationUsingKeyFrames;
                    ta.KeyFrames[0].Value = new Thickness(200, value.Top, -200, value.Bottom);
                    ta.KeyFrames[1].Value = value;
                }
            }
            sb.Begin();
        }

        private Grid LastPage;
        private void NSPage(Grid page) {
            SearchPage.Visibility = Visibility.Collapsed;
            page.Visibility = Visibility.Visible;
            LastPage = page;
            RunAnimation(page);
            ShowBackBtn.Begin();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Closing += MainWindow_Closing;
            ShowBackBtn = Resources["ShowBackBtn"] as Storyboard;
            LoadBingImg();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Save();
        }
        #region Search
        private int BingImgIndex = 0;
        /// <summary>
        /// Load BingPic 
        /// </summary>
        /// <param name="index">index:0~7</param>
        private async void LoadBingImg()
        {
            var data = await HttpHelper.GetWebAsync($"https://www.bing.com/HPImageArchive.aspx?format=js&idx={BingImgIndex}&n=1&mkt=zh-CN");
            JObject obj = JObject.Parse(data);
            this.Background = new ImageBrush(new BitmapImage(new Uri("https://www.bing.com" + obj["images"][0]["url"]))) { Stretch = Stretch.UniformToFill };
            BgImg_Url.Text = obj["images"][0]["copyright"].ToString();
            BgImg_Url.Uid = obj["images"][0]["copyrightlink"].ToString();
        }

        private void BgImg_LastBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (BingImgIndex < 7)
            {
                BingImgIndex++;
                LoadBingImg();
            }
        }

        private void BgImg_NextBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (BingImgIndex > 0)
            {
                BingImgIndex--;
                LoadBingImg();
            }
        }

        private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchBox.Text != "")
            {
                if (SearchBox.Text != "搜索")
                {
                    var data = await HttpHelper.GetWebAsync("https://www.baidu.com/sugrec?pre=1&p=3&ie=utf-8&json=1&prod=pc&from=pc_web&wd=" + Uri.EscapeDataString(SearchBox.Text).Replace("#", "%23") + "&req=2&pwd=ta");
                    Console.WriteLine(data);
                    JObject obj = JObject.Parse(data);
                    var aa = obj["g"];
                    SearchSugBox.Items.Clear();
                    if (aa==null)
                        return;
                    foreach (var item in aa)
                        SearchSugBox.Items.Add(new ListBoxItem() { Content = item["q"], Height = 35,Padding=new Thickness(10,0,10,0) });
                }
            }
            else SearchSugBox.Items.Clear();
        }

        private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
                SearchSugBox.Focus();
        }

        private void SearchSugBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (SearchSugBox.SelectedItem != null)
            {
                SearchBox.Text = (SearchSugBox.SelectedItem as ListBoxItem).Content.ToString();
            }
        }

        private void SearchSugBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SearchSugBox.SelectedItem != null)
            {
                SearchBox.Text = (SearchSugBox.SelectedItem as ListBoxItem).Content.ToString();
            }
        }
        #endregion

        private void WeatherBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NSPage(WeatherPage);
        }

        private void BackBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LastPage.Visibility = Visibility.Collapsed;
            SearchPage.Visibility = Visibility.Visible;
            LastPage = null;
            RunAnimation(SearchPage);
            ShowBackBtn.Stop();
        }
    }
}
