using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SeewoHelper
{
    /// <summary>
    /// DesktopWidget.xaml 的交互逻辑
    /// </summary>
    public partial class DesktopWidget : Window
    {
        string ApplyDefaultStyleScript = @"
        var root=document.querySelector("":root"")
        root.style.fontSize=""40px""
        root.style.fontWeight=""Bold""
        root.style.fontFamily=""微软雅黑""
        root.style.color=""white""
        
        var body=document.querySelector(""body"")
        body.style.backgroundColor=""rgba(0, 0, 0, 0.1)""

        var pre=document.getElementsByTagName(""pre"")[0]
        pre.style.fontSize=""40px""
        pre.style.fontWeight=""Bold""
        pre.style.fontFamily=""微软雅黑""
        pre.style.color=""white""";
        public DesktopWidget()
        {
            InitializeComponent();
        }

        private void Cover_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception)
            {
            }
        }

        private void Cover_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Cover_Drop(object sender, DragEventArgs e)
        {
            Browser.Address = (e.Data.GetData(DataFormats.FileDrop) as string[])[0];
        }


        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    if (Browser.Address != null)
                    {
                        string filePath = WebUtility.UrlDecode(Browser.Address.Substring(8));
                        string htmlContent = File.ReadAllText(filePath);
                        if (!htmlContent.Contains("UseCustomStyle", StringComparison.OrdinalIgnoreCase))
                            Browser.ExecuteScriptAsync(ApplyDefaultStyleScript);
                    }
                }
                catch (Exception)
                {
#if DEBUG
                    throw;
#endif
                }
                
            });
            
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Browser.GetBrowser().Reload();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SetTopMost_Checked(object sender, RoutedEventArgs e)
        {
            Topmost= true;
        }

        private void SetTopMost_Unchecked(object sender, RoutedEventArgs e)
        {
            Topmost= false;
        }

        private void SwitchView_Checked(object sender, RoutedEventArgs e)
        {
            BrowserGrid.Visibility = Visibility.Hidden;
            EditorGrid.Visibility = Visibility.Visible;
        }

        private void SwitchView_Unchecked(object sender, RoutedEventArgs e)
        {
            BrowserGrid.Visibility= Visibility.Visible;
            EditorGrid.Visibility= Visibility.Hidden;
        }
    }
}
