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

namespace Dailive.Pages
{
    /// <summary>
    /// AppsGoto.xaml 的交互逻辑
    /// </summary>
    public partial class AppsGoto : UserControl
    {
        public AppsGoto()
        {
            InitializeComponent();
        }
        public Geometry Data { get => icon.Data; set => icon.Data = value; }
        public string Txt { get => name.Text; set => name.Text = value; }
    }
}
