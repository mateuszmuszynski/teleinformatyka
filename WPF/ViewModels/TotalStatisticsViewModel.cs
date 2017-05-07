using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System.ComponentModel;

namespace WPF.ViewModels
{
    public class TotalStatisticsViewModel : INotifyPropertyChanged
    {
        private SeriesCollection _httpStatusCodes;
        private SeriesCollection _httpMethods;

        private ulong _totalReceivedBytes;
        private ulong _totalSentBytes;
        private ulong _totalReceivedPackets;
        private ulong _totalSentPackets;

        public ulong TotalReceivedBytes
        {
            get
            {
                return _totalReceivedBytes;
            }
            set
            {
                _totalReceivedBytes = value;
                OnPropertyChanged(nameof(TotalReceivedBytes));
            }
        }

        public ulong TotalSentBytes
        {
            get
            {
                return _totalSentBytes;
            }
            set
            {
                _totalSentBytes = value;
                OnPropertyChanged(nameof(TotalSentBytes));
            }
        }

        public ulong TotalReceivedPackets
        {
            get
            {
                return _totalReceivedPackets;
            }
            set
            {
                _totalReceivedPackets = value;
                OnPropertyChanged(nameof(TotalReceivedPackets));
            }
        }

        public ulong TotalSentPackets
        {
            get
            {
                return _totalSentPackets;
            }
            set
            {
                _totalSentPackets = value;
                OnPropertyChanged(nameof(TotalSentPackets));
            }
        }

        public SeriesCollection HttpStatusCodes
        {
            get
            {
                return _httpStatusCodes;
            }
            set
            {
                _httpStatusCodes = value;
                OnPropertyChanged(nameof(HttpStatusCodes));
            }
        }

        public SeriesCollection HttpMethods
        {
            get
            {
                return _httpMethods;
            }
            set
            {
                _httpMethods = value;
                OnPropertyChanged(nameof(HttpMethods));
            }
        }

        public TotalStatisticsViewModel()
        {
            HttpStatusCodes = new SeriesCollection();
            HttpMethods = new SeriesCollection();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}