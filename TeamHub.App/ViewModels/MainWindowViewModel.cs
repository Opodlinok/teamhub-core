using System;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace TeamHub.App.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _projectName = string.Empty;
        private string _projectPath = string.Empty;

        public string ProjectName
        {
            get => _projectName;
            set
            {
                _projectName = value;
                OnPropertyChanged(nameof(ProjectName));
                OnPropertyChanged(nameof(CanCreateProject));
            }
        }

        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                _projectPath = value;
                OnPropertyChanged(nameof(ProjectPath));
                OnPropertyChanged(nameof(CanCreateProject));
            }
        }

        public bool CanCreateProject =>
            !string.IsNullOrWhiteSpace(ProjectName) &&
            !string.IsNullOrWhiteSpace(ProjectPath);

        public ICommand BrowseCommand { get; }
        public ICommand CreateCommand { get; }

        public MainWindowViewModel()
        {
            BrowseCommand = new RelayCommand(async _ =>
            {
                var dialog = new OpenFolderDialog();
                var window = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
                                 ?.MainWindow;
                var result = await dialog.ShowAsync(window!);
                if (!string.IsNullOrWhiteSpace(result))
                    ProjectPath = result;
            });

            CreateCommand = new RelayCommand(_ =>
            {
                // TODO: hook up Unity CLI creation
            }, _ => CanCreateProject);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) =>
            _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) =>
            _execute(parameter);

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
