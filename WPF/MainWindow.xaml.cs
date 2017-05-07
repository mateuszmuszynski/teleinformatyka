using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Configurations;
using Services;
using WPF.Models;
using WPF.ViewModels;
using LiveCharts.Wpf;
using Common.Enums;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PacketService _packetService;
        private Timer _timer;

        public MainWindow()
        {
            InitializeComponent();

            _packetService = new PacketService();
            var devices = _packetService.GetAllDevices();

            var vm = new UserInterfaceViewModel(devices.Select(x => new DeviceViewModel
            {
                Name = x.Name,
                Description = x.Description
            }).ToList());

            DataContext = vm;

            var allHttpMethods = Enum.GetValues(typeof(HttpMethod)).Cast<HttpMethod>();

            foreach (var httpMethod in allHttpMethods.Where(x => x != HttpMethod.Unknown))
            {
                ((UserInterfaceViewModel)DataContext).TotalStatistics.HttpMethods.Add(new PieSeries
                {
                    Title = httpMethod.ToString(),
                    Values = new ChartValues<long> { 0 }
                });
            }
        }


        private void StartGatheringStatisticsButton_OnClick(object sender, RoutedEventArgs e)
        {
            StartGatheringStatisticsButton.IsEnabled = false;
            StopGatheringStatisticsButton.IsEnabled = true;

            var deviceModel = ((UserInterfaceViewModel) DataContext).SelectedDevice;

            var device = _packetService.GetAllDevices().First(x => x.Name == deviceModel.Name);

            _packetService.StartGatheringStatistics(device, new TimeSpan(0, 0, 1, 0, 0));

            _timer = new Timer(1000);
            _timer.Elapsed += GetCurrentStatistics;
            _timer.Enabled = true;
        }

        private void GetCurrentStatistics(object sender, ElapsedEventArgs e)
        {
            var perSecondStatistics = _packetService.GetPerSecondStatistics();
            var totalStatistics = _packetService.GetTotalStatistics();

            Dispatcher.Invoke(() =>
            {
                var dataContext = ((UserInterfaceViewModel) DataContext);

                var now = DateTime.UtcNow;

                foreach (var totalStatistic in totalStatistics.HttpStatusCodesCount)
                {
                    var itemName = totalStatistic.Key.ToString();

                    var existingItem = dataContext.TotalStatistics.HttpStatusCodes.FirstOrDefault(x => ((PieSeries)x).Title == itemName);

                    if (existingItem != null)
                    {
                        ((ChartValues<long>)((PieSeries)existingItem).Values)[0] = (long)totalStatistic.Value;
                    }
                    else
                    {
                        dataContext.TotalStatistics.HttpStatusCodes.Add(new PieSeries
                        {
                            Title = itemName,
                            Values = new ChartValues<long> { (long)totalStatistic.Value }
                        });
                    }
                }

                dataContext.TotalStatistics.TotalReceivedBytes = totalStatistics.TotalReceivedBytes;
                dataContext.TotalStatistics.TotalSentBytes = totalStatistics.TotalSentBytes;
                dataContext.TotalStatistics.TotalReceivedPackets = totalStatistics.TotalReceivedPackets;
                dataContext.TotalStatistics.TotalSentPackets = totalStatistics.TotalSentPackets;

                    foreach (var totalStatistic in totalStatistics.MethodsCount)
                    {
                        var itemName = totalStatistic.Key.ToString();

                        var existingItem = dataContext.TotalStatistics.HttpMethods.FirstOrDefault(x => ((PieSeries)x).Title == itemName);

                        ((ChartValues<long>)((PieSeries)existingItem).Values)[0] = (long)totalStatistic.Value;
                    }

                var lastReceivedItem = dataContext.BytesChart.ReceivedItems.LastOrDefault();

                var receivedStatisticsToShow = perSecondStatistics.Where(x => lastReceivedItem == null || x.Time > lastReceivedItem.Time).ToList();

                if (!receivedStatisticsToShow.Any())
                {
                    dataContext.BytesChart.ReceivedItems.Add(new BytesChartItemViewModel
                    {
                        Time = now,
                        Bytes = 0
                    });
                }
                else
                {
                    dataContext.BytesChart.ReceivedItems.AddRange(
                        receivedStatisticsToShow.Select(x => new BytesChartItemViewModel
                        {
                            Bytes = x.ReceivedBytes,
                            Time = x.Time
                        }).ToList());
                }

                var lastSentItem = dataContext.BytesChart.SentItems.LastOrDefault();

                var sentStatisticsToShow = perSecondStatistics.Where(x => lastSentItem == null || x.Time > lastSentItem.Time).ToList();

                if (!sentStatisticsToShow.Any())
                {
                    dataContext.BytesChart.SentItems.Add(new BytesChartItemViewModel
                    {
                        Time = now,
                        Bytes = 0
                    });
                }
                else
                {
                    dataContext.BytesChart.SentItems.AddRange(
                        sentStatisticsToShow.Select(x => new BytesChartItemViewModel
                        {
                            Bytes = x.SentBytes,
                            Time = x.Time
                        }).ToList());
                }

                dataContext.BytesChart.SetAxisLimits(now);
            });
        }

        private void StopGatheringStatisticsButton_OnClick(object sender, RoutedEventArgs e)
        {
            StartGatheringStatisticsButton.IsEnabled = true;
            StopGatheringStatisticsButton.IsEnabled = false;

            _packetService.StopGatheringStatistics();
            _timer.Enabled = false;
        }
    }
}
