using CefSharp.DevTools.Page;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using static SeewoHelper.Schedule;

namespace SeewoHelper
{
    /// <summary>
    /// ScheduleEditor.xaml 的交互逻辑
    /// </summary>
    public partial class ScheduleEditor : Window
    {

        public ScheduleEditor()
        {
            InitializeComponent();
            DataContext = new ScheduleEditorViewModel();
        }
    }

    public class ScheduleEditorViewModel : INotifyPropertyChanged
    {
        public JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new TimeOnlyConverter()
            }
        };
        public ICommand WindowLoadedCommand { get; set; }
        public ICommand ScheduleFilesListSelectionChangedCommand { get; set; }
        public ICommand LoadExternalScheduleFileCommand { get; set; }
        public ICommand DataGridCellEditEndCommand { get; set; }
        public ICommand SaveFileCommand { get; set; }
        public ICommand DeleteRowCommand { get; set; }
        public ICommand AddRowCommand { get; set; }
        private ObservableCollection<string> _scheduleFiles = new();
        public ObservableCollection<string> ScheduleFiles
        {
            get { return _scheduleFiles; }
            set { _scheduleFiles = value; OnPropertyChanged(); }
        }
        private ObservableCollection<Curriculum> _workingCurricula = new();
        public ObservableCollection<Curriculum> WorkingCurricula
        {
            get => _workingCurricula;
            set { _workingCurricula = value; OnPropertyChanged(); }
        }
        private string _selectedScheduleFilePath = "";
        public string SelectedScheduleFilePath
        {
            get => _selectedScheduleFilePath;
            set { _selectedScheduleFilePath = value; OnPropertyChanged(); }
        }
        private int _selectedScheduleIndex = -1;
        public int SelectedScheduleIndex
        {
            get => _selectedScheduleIndex;
            set { _selectedScheduleIndex = value; OnPropertyChanged(); }
        }

        private string _workingScheduleFile = "";
        public string WorkingScheduleFile
        {
            get => _workingScheduleFile;
            set { _workingScheduleFile = value; OnPropertyChanged(); }
        }
        public Curriculum SelectedRow { get; set; } = null;
        public ScheduleEditorViewModel()
        {
            WindowLoadedCommand = new CustomCommand(WindowLoaded, ReturnCanExecute);
            ScheduleFilesListSelectionChangedCommand = new CustomCommand(SelectionChanged, ReturnCanExecute);
            LoadExternalScheduleFileCommand = new CustomCommand(LoadExternalScheduleFile, ReturnCanExecute);
            DataGridCellEditEndCommand = new CustomCommand(CellEditEnd, ReturnCanExecute);
            SaveFileCommand = new CustomCommand(SaveFile, IsSaveEnabled);
            DeleteRowCommand = new CustomCommand(DeleteRow, IsDeleteRowEnabled);
            AddRowCommand = new CustomCommand(AddRow, ReturnCanExecute);
            WorkingScheduleFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Schedules", "NewSchedule.json");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void WindowLoaded(object parameter)
        {
            if (!Directory.Exists("Schedules"))
            {
                Directory.CreateDirectory("Schedules");
            }
            DirectoryInfo schedulesDirectory = new DirectoryInfo("Schedules");
            FileInfo[] fileInfos = schedulesDirectory.EnumerateFiles().ToArray();
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (string.Equals(fileInfo.Extension, ".json", System.StringComparison.OrdinalIgnoreCase))
                {
                    ScheduleFiles.Add(fileInfo.FullName);
                }
            }
        }
        public void LoadScheduleFile()
        {
            CommandManager.InvalidateRequerySuggested();
            SelectedRow = null;
            WorkingCurricula.Clear();
            string jsonContent = File.ReadAllText(WorkingScheduleFile);
            WorkingCurricula = new ObservableCollection<Curriculum>(JsonSerializer.Deserialize<Curriculum[]>(jsonContent, JsonSerializerOptions));
            WorkingCurricula = new ObservableCollection<Curriculum>(WorkingCurricula.OrderBy((i) => i.StartTime));
        }

        public void LoadExternalScheduleFile(object parameter)
        {
            var ofd = new OpenFileDialog();
            ofd.DefaultExt = "(*.json)|*.json";
            ofd.Multiselect = false;
            if (!(ofd.ShowDialog() ?? false))
                return;
            WorkingScheduleFile = ofd.FileName;
            ScheduleFiles.Add(WorkingScheduleFile);
            SelectedScheduleFilePath = WorkingScheduleFile;
            LoadScheduleFile();
        }

        public void SelectionChanged(object parameter)
        {
            WorkingScheduleFile = SelectedScheduleFilePath;
            LoadScheduleFile();
        }
        public void CellEditEnd(object parameter)
        {
            WorkingCurricula=new ObservableCollection<Curriculum>(WorkingCurricula.OrderBy((i) => i.StartTime));
        }
        public void SaveFile(object parameter)
        {
            var svd = new SaveFileDialog();
            svd.Filter = "(*.json)|*.json";
            svd.DefaultExt = "(*.json)|*.json";
            svd.AddExtension = true;
            FileInfo defaultFileInfo = new FileInfo(WorkingScheduleFile);
            svd.FileName = defaultFileInfo.Name;
            svd.InitialDirectory = defaultFileInfo.DirectoryName;
            if (!(svd.ShowDialog() ?? false))
                return;
            string jsonContent = JsonSerializer.Serialize(WorkingCurricula.OrderBy((i)=>i.StartTime),JsonSerializerOptions);
            File.WriteAllText(svd.FileName,jsonContent);
        }
        public void DeleteRow(object parameter)
        {
            WorkingCurricula.Remove(SelectedRow);
        }
        public void AddRow(object parameter)
        {
            WorkingCurricula.Add(new Curriculum("", TimeOnly.Parse("23:59")));
        }
        public bool ReturnCanExecute(object parameter)
        {
            return true;
        }
        public bool IsSaveEnabled(object parameter)
        {
            if (string.IsNullOrEmpty(WorkingScheduleFile))
                return false;
            return true;
        }
        public bool IsDeleteRowEnabled(object parameter)
        {
            if (WorkingCurricula == null)
                return false;
            else return true;
        }
    }
    public class CustomCommand : ICommand
    {
        private readonly Action<object> _commandAction;
        private readonly Func<object, bool> _changeCanExecute;
        public CustomCommand(Action<object> commandAction, Func<object, bool> changeCanExecute)
        {
            _commandAction = commandAction;
            _changeCanExecute = changeCanExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _changeCanExecute.Invoke(parameter);
        }

        public void Execute(object? parameter)
        {
            _commandAction.Invoke(parameter);
        }
    }
    public class TimeOnlyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TimeOnly)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return TimeOnly.Parse(value.ToString());
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return new TimeOnly();
            }
        }
    }
    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return TimeOnly.Parse(reader.GetString());
            }
            catch (Exception)
            {
                return new TimeOnly();
            }
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
