using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using static SeewoHelper.ScheduleEditor;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace SeewoHelper
{
    /// <summary>
    /// Schedule.xaml 的交互逻辑
    /// </summary>
    public partial class Schedule : Window
    {
        public int CurrentClassIndex;
        public Storyboard MoveToNextClassStoryboard;
        //public Timer GlobalTimer = new Timer();
        public DispatcherTimer GlobalTimer = new DispatcherTimer();
        public Curriculum[] Curricula;
        public JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = { new TimeOnlyConverter()}
        };

        public class Curriculum
        {
            public Curriculum(string name, TimeOnly startTime)
            {
                Name = name;
                StartTime = startTime;
            }
            public Curriculum()
            {

            }
            public string Name { get; set; }
            public TimeOnly StartTime { get; set; }

        }

        public void InitializeScheduel()
        {
            GlobalTimer.Stop();
            TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);
            for (int i = 0; i <= Curricula.Length - 1; i++)
            {
                if (Curricula[i].StartTime > now)
                {
                    string prevClassName = "";
                    string currentClassName = "";
                    string nextClassName = "";
                    if (i >= 2)
                        prevClassName = Curricula[i - 2].Name;
                    if (i >= 1)
                        currentClassName = Curricula[i - 1].Name;
                    PreviousClass.Text = prevClassName;
                    CurrentClass.Text = currentClassName;

                    if (i <= Curricula.Length - 1)
                    {
                        NextClass.Text = Curricula[i].Name;
                        GlobalTimer.Interval = Curricula[i].StartTime - now;
                        GlobalTimer.Tick += GlobalTimer_Tick;
                        GlobalTimer.Start();
                    }

                    else
                        NextClass.Text = "";
                    if (i <= Curricula.Length - 2)
                        PlaceHolder.Text = Curricula[i + 1].Name;
                    else
                        PlaceHolder.Text = "";
                    CurrentClassIndex = i - 1;


                    return;
                }
            }

            PreviousClass.Text = "";
            CurrentClass.Text = "";
            NextClass.Text = "";
            PlaceHolder.Text = "";
            if (Curricula.Length >= 1)
                CurrentClass.Text = Curricula.Last().Name;
            if (Curricula.Length >= 2)
                PreviousClass.Text = Curricula[Curricula.Length - 2].Name;
        }
        public Schedule()
        {

            if (!Directory.Exists("Schedules"))
                Directory.CreateDirectory("Schedules");
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            InitializeComponent();

            Left = System.Windows.SystemParameters.WorkArea.Width - Width - 12;
            Top = 12;

            MoveToNextClassStoryboard = Resources["MoveToNextClass"] as Storyboard;
            MoveToNextClassStoryboard.FillBehavior = FillBehavior.Stop;
            MoveToNextClassStoryboard.Completed += Storyboard_Completed;
            try
            {
                string jsonData = File.ReadAllText("Schedules\\" + today.DayOfWeek.ToString() + ".json");
                /*
                DeCurriculum[] dc = JsonSerializer.Deserialize<DeCurriculum[]>(jsonData);
                List<Curriculum> tempCurricula = new();
                foreach (var item in dc)
                {
                    tempCurricula.Add(new Curriculum(item.Name, TimeOnly.Parse(item.StartTime)));
                }
                Curricula = tempCurricula.ToArray();*/
                Curricula = JsonSerializer.Deserialize<Curriculum[]>(jsonData, JsonSerializerOptions);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"错误：\n{ex.Message}\n请手动指定课程文件");
                Curricula = Array.Empty<Curriculum>();
            }

            InitializeScheduel();
        }

        private void GlobalTimer_Tick(object? sender, EventArgs e)
        {
            MoveToNextClassStoryboard.Begin();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            {

            }

        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            BeginStoryboard(Resources["ShowBackgroundRect"] as Storyboard);
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            BeginStoryboard(Resources["HideBackgroundRect"] as Storyboard);
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //MoveToNextClassStoryboard.Begin();
        }

        private void Storyboard_Completed(object? sender, EventArgs e)
        {
            GlobalTimer.Stop();
            CurrentClassIndex++;
            PreviousClass.Text = CurrentClass.Text;
            CurrentClass.Text = NextClass.Text;
            NextClass.Text = PlaceHolder.Text;
            if (CurrentClassIndex <= Curricula.Length - 3)
                PlaceHolder.Text = Curricula[CurrentClassIndex + 2].Name;
            else
                PlaceHolder.Text = "";
            if (CurrentClassIndex <= Curricula.Length - 2)
            {
                GlobalTimer = new DispatcherTimer();
                GlobalTimer.Interval = Curricula[CurrentClassIndex + 1].StartTime - TimeOnly.FromDateTime(DateTime.Now);
                GlobalTimer.Tick += GlobalTimer_Tick;
                GlobalTimer.Start();
            }

            IReadOnlyList<WindowInfo> windows = WindowEnumerator.FindAll();
            var presentationSource = PresentationSource.FromVisual(this);
            double scale = 1.0;
            if (presentationSource != null)
                scale = presentationSource.CompositionTarget.TransformToDevice.M11;

            foreach (WindowInfo window in windows)
            {
                if (window.Bounds.Right >= System.Windows.SystemParameters.WorkArea.Width * scale)
                {
                    FullScreenSchedule fss = new FullScreenSchedule(new System.Collections.ObjectModel.ObservableCollection<Curriculum>(Curricula));
                    fss.Show();
                    break;
                }
            }
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
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

        private void Window_Drop(object sender, DragEventArgs e)
        {
            try
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);
                var fileName = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                var info = new FileInfo(fileName);
                if (info?.Extension.ToLower() != ".json")
                    return;
                string jsonData = File.ReadAllText(fileName);
                /*
                DeCurriculum[] dc = JsonSerializer.Deserialize<DeCurriculum[]>(jsonData);
                List<Curriculum> tempCurricula = new();
                foreach (var item in dc)
                {
                    tempCurricula.Add(new Curriculum(item.Name, TimeOnly.Parse(item.StartTime)));
                }
                Curricula = tempCurricula.ToArray();*/
                Curricula = JsonSerializer.Deserialize<Curriculum[]>(jsonData, JsonSerializerOptions);
                InitializeScheduel();
                if (
                    MessageBox.Show("成功载入课程表，是否将其设定为今日默认课表？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
                {
                    File.WriteAllText(today.DayOfWeek.ToString() + ".json", jsonData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"错误",MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                Curricula = Array.Empty<Curriculum>();
            }
        }

        private void OpenSchedulesFolder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("explorer.exe", Path.Combine(Environment.CurrentDirectory, "Schedules"));
        }

        private void CloseWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(CurrentClass.Text!="生物")
                this.Close();
            else
            {
                MessageBox.Show("可人不准关", "嘻嘻", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void EditSchedule_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (new ScheduleEditor()).Show();
        }

        private void SetTopMost_Checked(object sender, RoutedEventArgs e)
        {
            Topmost= true;
#if DEBUG
            new FullScreenSchedule(new System.Collections.ObjectModel.ObservableCollection<Curriculum>(Curricula)).Show();
#endif     
        }

        private void SetTopMost_Unchecked(object sender, RoutedEventArgs e)
        {
            Topmost= false;

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            GlobalTimer.Stop();
            base.OnClosing(e);
        }
    }
}
