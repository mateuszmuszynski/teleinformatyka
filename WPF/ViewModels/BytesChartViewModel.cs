using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Configurations;
using WPF.Annotations;

namespace WPF.ViewModels
{
    public class BytesChartViewModel : INotifyPropertyChanged
    {
        public ChartValues<BytesChartItemViewModel> ReceivedItems { get; set; }
        public ChartValues<BytesChartItemViewModel> SentItems { get; set; }

        public BytesChartViewModel()
        {
            ReceivedItems = new ChartValues<BytesChartItemViewModel>();
            SentItems = new ChartValues<BytesChartItemViewModel>();

            AxisStep = TimeSpan.FromSeconds(1).Ticks;
            AxisUnit = 0;

            var mapper =
    Mappers.Xy<BytesChartItemViewModel>().X(model => model.Time.Ticks).Y(model => model.Bytes );

            Charting.For<BytesChartItemViewModel>(mapper);

            SetAxisLimits(DateTime.Now);
        }

        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged(nameof(AxisMax));
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged(nameof(AxisMin));
            }
        }

        public void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(60).Ticks; // and 60 seconds behind
        }

        private double _axisMax;
        private double _axisMin;

        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
