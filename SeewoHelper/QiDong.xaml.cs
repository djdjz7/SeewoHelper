using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SeewoHelper
{
    /// <summary>
    /// QiDong.xaml 的交互逻辑
    /// </summary>
    public partial class QiDong : Window
    {
        QiDongMode mode;
        public QiDong(QiDongMode mode)
        {
            InitializeComponent();
            this.mode = mode;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (mode)
            {
                case QiDongMode.None:
                    this.Close();
                    break;
                case QiDongMode.Genshin:
                    break;
                case QiDongMode.EasiNote:
                    this.FontFamily = new FontFamily("Microsoft Yahei");
                    this.FontWeight = FontWeights.Bold;
                    this.GenshinLogo.Source = Application.Current.Resources["EasiNoteLogo"] as DrawingImage;
                    this.GenshinLogo.Height = 128;
                    this.MihoyoLogo.Source = Application.Current.Resources["SeewoLogo"] as DrawingImage;
                    this.MihoyoLogo.Height = 128;
                    this.Numbers.Text = "粤ICP备12092924号";
                    this.Copyrighter.Text = "著作权人：广州视睿电子科技有限公司";
                    this.WarningTitle.Text = "警告：课前详阅";
                    break;
                case QiDongMode.RealMadrid:
                    this.FontFamily = new FontFamily("Microsoft Yahei");
                    this.FontWeight = FontWeights.Bold;
                    this.GenshinLogo.Source = Application.Current.Resources["RealMadridLogo"] as DrawingImage;
                    this.GenshinLogo.Height = 200;
                    this.MihoyoLogo.Source = Application.Current.Resources["UEFALogo"] as DrawingImage;
                    this.MihoyoLogo.Height = 200;
                    this.ExtraInfo.Visibility = Visibility.Collapsed;
                    this.WarningTitle.Text = "警告：课前详阅";
                    break;
                default:
                    break;
            }
            var QiDongAnimation = Resources[mode==QiDongMode.Genshin?"QiDongAnimation":"QiDongAnimationShort"] as Storyboard;
            if (QiDongAnimation == null)
                this.Close();
            QiDongAnimation.Completed += (sender, e) => this.Close();
            QiDongAnimation.Begin();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }
    }

    public enum QiDongMode
    {
        EasiNote,
        None,
        Genshin,
        RealMadrid,
    }
}
