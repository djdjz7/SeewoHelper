using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace SeewoHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string EasiNote5Location = "";
        string CameraLocation = "";
        string ZyflierLocation = "";
        bool IsStartAvailable = true;
        int modeCounter = 0;
        private DispatcherTimer weatherTimer = new DispatcherTimer();
        private string weatherLocationCode = "";

        DateTime LastClick = DateTime.MinValue;
        int ClickCount = 0;

        public MainWindow()
        {
            if (File.Exists("WeatherLocationCode"))
                weatherLocationCode = File.ReadAllText("WeatherLocationCode");
            else
                File.Create("WeatherLocationCode");
            weatherLocationCode = weatherLocationCode.Trim();
            InitializeComponent();
            try
            {
                RegistryKey easinoteKey = Registry.ClassesRoot.OpenSubKey("easinote");
                EasiNote5Location = easinoteKey.GetValue("URL Protocol").ToString();
                easinoteKey.Close();
            }
            catch
            {
                EasiNote5Location = "";
            }

            try
            {
                RegistryKey cameraKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Seewo\\EasiCamera");
                CameraLocation = "";
                if (cameraKey != null)
                {
                    CameraLocation = cameraKey.GetValue("path").ToString().Replace(@"\\", @"\") + @"\sweclauncher\sweclauncher.exe";
                    cameraKey.Close();
                }

            }
            catch
            {
                CameraLocation = "";
            }

            try
            {
                RegistryKey zyflierKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\中育飞车助手");
                ZyflierLocation = zyflierKey.GetValue("DisplayIcon").ToString();
            }
            catch
            {
                ZyflierLocation = "";
            }

            new Schedule().Show();

            if (!string.IsNullOrEmpty(weatherLocationCode))
            {
                weatherTimer.Interval = new TimeSpan(0, 10, 0);
                weatherTimer.Tick += WeatherTimer_Tick;
                weatherTimer.Start();
            }
            else
                QweatherPanel.Visibility = Visibility.Collapsed;
        }

        private async void WeatherTimer_Tick(object? sender, EventArgs e)
        {
            await UpdateWeatherInfo();
        }

        private void EasiNote5Icon_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DoubleAnimation en5DepthAnim = new DoubleAnimation()
            {
                To = 8,
                Duration = TimeSpan.FromSeconds(0.2),
                AccelerationRatio = 0,
                DecelerationRatio = 1
            };
            DoubleAnimation en5BlurAnim = new DoubleAnimation()
            {
                To = 32,
                Duration = TimeSpan.FromSeconds(0.2),
                AccelerationRatio = 0,
                DecelerationRatio = 1
            };
            EasiNote5IconShadow.BeginAnimation(DropShadowEffect.ShadowDepthProperty, en5DepthAnim);
            EasiNote5IconShadow.BeginAnimation(DropShadowEffect.BlurRadiusProperty, en5BlurAnim);
        }

        private void EasiNote5Icon_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DoubleAnimation en5DepthAnim = new DoubleAnimation()
            {
                To = 4,
                Duration = TimeSpan.FromSeconds(0.2),
                AccelerationRatio = 0,
                DecelerationRatio = 1
            };
            DoubleAnimation en5BlurAnim = new DoubleAnimation()
            {
                To = 8,
                Duration = TimeSpan.FromSeconds(0.2),
                AccelerationRatio = 0,
                DecelerationRatio = 1
            };
            EasiNote5IconShadow.BeginAnimation(DropShadowEffect.ShadowDepthProperty, en5DepthAnim);
            EasiNote5IconShadow.BeginAnimation(DropShadowEffect.BlurRadiusProperty, en5BlurAnim);

        }

        private void CameraIcon_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DoubleAnimation cameraDepthAnim = new DoubleAnimation()
            {
                To = 8,
                Duration = TimeSpan.FromSeconds(0.2),
                AccelerationRatio = 0,
                DecelerationRatio = 1
            };
            DoubleAnimation cameraBlurAnim = new DoubleAnimation()
            {
                To = 32,
                Duration = TimeSpan.FromSeconds(0.2),
                AccelerationRatio = 0,
                DecelerationRatio = 1
            };
            CameraIconShadow.BeginAnimation(DropShadowEffect.ShadowDepthProperty, cameraDepthAnim);
            CameraIconShadow.BeginAnimation(DropShadowEffect.BlurRadiusProperty, cameraBlurAnim);
        }

        private void CameraIcon_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DoubleAnimation cameraDepthAnim = new DoubleAnimation()
            {
                To = 4,
                Duration = TimeSpan.FromSeconds(0.2),
                AccelerationRatio = 0,
                DecelerationRatio = 1
            };
            DoubleAnimation cameraBlurAnim = new DoubleAnimation()
            {
                To = 8,
                Duration = TimeSpan.FromSeconds(0.2),
                AccelerationRatio = 0,
                DecelerationRatio = 1
            };
            CameraIconShadow.BeginAnimation(DropShadowEffect.ShadowDepthProperty, cameraDepthAnim);
            CameraIconShadow.BeginAnimation(DropShadowEffect.BlurRadiusProperty, cameraBlurAnim);
        }

        private void EasiNote5Icon_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsStartAvailable)
            {
                IsStartAvailable = false;
                new QiDong((QiDongMode)modeCounter).Show();
                Storyboard openAnim = Resources["OpenEasiNote5"] as Storyboard;
                openAnim.AutoReverse = true;
                openAnim.Begin();
                try
                {
                    Process.Start(EasiNote5Location);
                }
                catch { }
            }

        }

        private void OpenAnim_Completed(object? sender, EventArgs e)
        {
            IsStartAvailable = true;
        }

        private void CameraIcon_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsStartAvailable)
            {
                IsStartAvailable = false;
                Storyboard openAnim = Resources["OpenCamera"] as Storyboard;
                openAnim.AutoReverse = true;
                openAnim.Begin();
                try
                {
                    Process.Start(CameraLocation);
                }
                catch { }
            }
        }

        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DateTime.Now - LastClick <= TimeSpan.FromMilliseconds(250))
            {
                ClickCount++;
                LastClick = DateTime.Now;
            }
            else
            {
                ClickCount = 1;
                LastClick = DateTime.Now;
            }

            if (ClickCount >= 3)
                Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource? hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
            DriveList = DriveInfo.GetDrives().ToList();
        }

        public const int WM_DEVICECHANGE = 0x219;//U盘插入后，OS的底层会自动检测到，然后向应用程序发送“硬件设备状态改变“的消息
        public const int DBT_DEVICEARRIVAL = 0x8000;  //就是用来表示U盘可用的。一个设备或媒体已被插入一块，现在可用。
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;  //一个设备或媒体片已被删除。

        List<DriveInfo> DriveList = new List<DriveInfo>();
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            try
            {
                if (msg == WM_DEVICECHANGE)
                {
                    switch (wParam.ToInt32())
                    {
                        case DBT_DEVICEARRIVAL:
                            foreach (DriveInfo info in DriveInfo.GetDrives())
                            {
                                if (!ContainsDrive(DriveList, info))
                                {
                                    USBToShowList.list.Add(info);
                                    if (USBToShowList.list.Count <= 1)
                                    {
                                        new USBDrivePopUp().Show();
                                    }
                                }
                            }
                            DriveList = DriveInfo.GetDrives().ToList();
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:
                            DriveList = DriveInfo.GetDrives().ToList();
                            break;
                        default:
                            break;
                    }
                }

            }

            catch
            {

            }
            return IntPtr.Zero;
        }

        private static bool ContainsDrive(List<DriveInfo> list, DriveInfo driveInfo)
        {
            foreach (DriveInfo d in list)
            {
                if (d.DriveType == driveInfo.DriveType &&
                    d.Name == driveInfo.Name &&
                    d.VolumeLabel == driveInfo.VolumeLabel)
                    return true;
            }
            return false;
        }

        private void ShowSchedule_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Schedule schedule = new Schedule();
            schedule.Show();
        }

        private void ExitApplication_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenWidget_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new DesktopWidget().Show();
        }

        private void ModeCounter_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            modeCounter = (modeCounter + 1) % 4;
            switch (modeCounter)
            {
                case 0:
                    ModeCounter.Text = "·";
                    break;
                case 1:
                    ModeCounter.Text = "·\n·";
                    break;
                case 2:
                    ModeCounter.Text = "·\n·\n·";
                    break;
                case 3:
                    ModeCounter.Text = "·\n·\n·\n·";
                    break;
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            weatherTimer.Stop();
            base.OnClosing(e);
        }
        private async Task UpdateWeatherInfo()
        {
            try
            {
                var airQuality = (await QWeatherAPI.GetAirQualityAsync(Confidentials.WeatherAPIKey, weatherLocationCode))?.now;
                if (airQuality is not null)
                {
                    AQIValueText.Text = "AQI: " + airQuality.aqi;
                    AQISlider.Value = double.Parse(airQuality.aqi);
                    AirQualityCategory.Text = airQuality.category;
                }
                var CurrentWeather = (await QWeatherAPI.GetCurrentWeatherAsync(Confidentials.WeatherAPIKey, weatherLocationCode))?.now;
                if (CurrentWeather is not null)
                {
                    Temperature.Text = CurrentWeather.temp + "℃";
                    WeatherText.Text = CurrentWeather.text;
                }
                QweatherPanel.Visibility = Visibility.Visible;
            }
            catch
            {
                QweatherPanel.Visibility = Visibility.Collapsed;
            }
        }

        private async void window_ContentRendered(object sender, EventArgs e)
        {
            Left = System.Windows.SystemParameters.WorkArea.Width - ActualWidth - 12;
            Top = System.Windows.SystemParameters.WorkArea.Height - ActualHeight - 12;
            await UpdateWeatherInfo();
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Left = System.Windows.SystemParameters.WorkArea.Width - ActualWidth - 12;
            Top = System.Windows.SystemParameters.WorkArea.Height - ActualHeight - 12;
        }
    }
}
