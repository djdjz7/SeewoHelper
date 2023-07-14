using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SeewoHelper
{
    /// <summary>
    /// USBDrivePopUp.xaml 的交互逻辑
    /// </summary>
    public partial class USBDrivePopUp : Window
    {
        public USBDrivePopUp()
        {
            try
            {

                InitializeComponent();
            }
            catch
            {

            }


        }
        DispatcherTimer timer = new DispatcherTimer();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Top = System.Windows.SystemParameters.WorkArea.Height - Height - 12;
            List<DriveInfo> drivesToRemove = new();
            foreach (var i in USBToShowList.list)
            {
                if (i.IsReady == false)
                    drivesToRemove.Add(i);
            }
            foreach (var drive in drivesToRemove)
            {
                USBToShowList.list.Remove(drive);
            }
            if (USBToShowList.list.Count == 0)
            {
                this.Close();
                return;
            }
            DriveNameLabel.Text = $"{USBToShowList.list[0].VolumeLabel} ({USBToShowList.list[0].Name})";

            BeginStoryboard(Resources["Show"] as Storyboard);
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            timer.Stop();
            Storyboard closeStoryboard = Resources["Close"] as Storyboard;
            closeStoryboard.Completed += CloseStoryboard_Completed;
            BeginStoryboard(closeStoryboard);
        }

        private void CloseStoryboard_Completed(object? sender, EventArgs e)
        {
            USBToShowList.list.RemoveAt(0);
            if (USBToShowList.list.Count > 0)
            {
                USBDrivePopUp popUp = new USBDrivePopUp();
                popUp.Show();
            }
            this.Close();
        }

        private void grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("explorer", USBToShowList.list[0].Name);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }
    }
}
