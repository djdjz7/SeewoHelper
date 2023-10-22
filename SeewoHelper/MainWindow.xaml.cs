using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

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

        DateTime LastClick = DateTime.MinValue;
        int ClickCount = 0;

        public MainWindow()
        {

            InitializeComponent();
            Left = System.Windows.SystemParameters.WorkArea.Width - Width - 12;
            Top = System.Windows.SystemParameters.WorkArea.Height - Height - 12;
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
            //if (string.IsNullOrEmpty(ZyflierLocation))
            //ZyflierIcon.Visibility = Visibility.Collapsed;

            new Schedule().Show();
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
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
            DriveList = DriveInfo.GetDrives().ToList();
        }

        public const int WM_DEVICECHANGE = 0x219;//U盘插入后，OS的底层会自动检测到，然后向应用程序发送“硬件设备状态改变“的消息
        public const int DBT_DEVICEARRIVAL = 0x8000;  //就是用来表示U盘可用的。一个设备或媒体已被插入一块，现在可用。
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;  //要求更改当前的配置（或取消停靠码头）已被取消。
        public const int DBT_CONFIGCHANGED = 0x0018;  //当前的配置发生了变化，由于码头或取消固定。
        public const int DBT_CUSTOMEVENT = 0x8006; //自定义的事件发生。 的Windows NT 4.0和Windows 95：此值不支持。
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;  //审批要求删除一个设备或媒体作品。任何应用程序也不能否认这一要求，并取消删除。
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;  //请求删除一个设备或媒体片已被取消。
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;  //一个设备或媒体片已被删除。
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;  //一个设备或媒体一块即将被删除。不能否认的。
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;  //一个设备特定事件发生。
        public const int DBT_DEVNODES_CHANGED = 0x0007;  //一种设备已被添加到或从系统中删除。
        public const int DBT_QUERYCHANGECONFIG = 0x0017;  //许可是要求改变目前的配置（码头或取消固定）。
        public const int DBT_USERDEFINED = 0xFFFF;  //此消息的含义是用户定义的
        public const uint GENERIC_READ = 0x80000000;
        public const int GENERIC_WRITE = 0x40000000;
        public const int FILE_SHARE_READ = 0x1;
        public const int FILE_SHARE_WRITE = 0x2;
        public const int IOCTL_STORAGE_EJECT_MEDIA = 0x2d4808;

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
            switch(modeCounter)
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
    }
}
