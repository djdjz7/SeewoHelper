using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using static SeewoHelper.Schedule;

namespace SeewoHelper
{
    /// <summary>
    /// FullScreenSchedule.xaml 的交互逻辑
    /// </summary>
    public partial class FullScreenSchedule : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Curriculum> _curricula;
        public ObservableCollection<Curriculum> Curricula
        {
            get { return _curricula; }
            set { _curricula = value;OnPropertyChanged(); }
        }
        public FullScreenSchedule(ObservableCollection<Curriculum> curricula)
        {
            InitializeComponent();
            Curricula = curricula;
            this.DataContext = this;
            Left = SystemParameters.WorkArea.Width - Width;
            Top=SystemParameters.WorkArea.Height/2 - MainListBox.Height/2;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 2, 0);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, System.EventArgs e)
        {
            this.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
