using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using LiveCharts;
using LiveCharts.Configurations;
using WPF.Annotations;
using WPF.Models;

namespace WPF.ViewModels
{
    public class UserInterfaceViewModel : INotifyPropertyChanged
    {
        public CollectionView Devices { get; }

        public BytesChartViewModel BytesChart { get; set; } 

        public TotalStatisticsViewModel TotalStatistics { get; set; }

        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }

        private DeviceViewModel _selectedDevice;

        public UserInterfaceViewModel(List<DeviceViewModel> devices)
        {
            Devices = new CollectionView(devices);

            BytesChart = new BytesChartViewModel();
            TotalStatistics = new TotalStatisticsViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
