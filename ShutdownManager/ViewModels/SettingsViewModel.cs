
using System.ComponentModel;

namespace ShutdownManager.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool AutoStartActiv { get => App.AppCon.AutoStart.AutoStartActiv; set => App.AppCon.AutoStart.ChangeAutoStartChecked(); }
    }
}
