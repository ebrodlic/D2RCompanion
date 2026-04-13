using D2RPriceChecker.Domain;
using D2RPriceChecker.Features.Traderie.DTO;
using D2RPriceChecker.Features.Traderie.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace D2RPriceChecker.ViewModels
{
    public class OverlayViewModel : INotifyPropertyChanged
    {
        // Top section: OCR results
        public ObservableCollection<string> OcrLines { get; set; } = new();

        // Bottom section: Trades info
        public ObservableCollection<Trade> Trades { get; set; } = new();

        // Mid section: Trade/prices statistics
        private TradeStatistics _statistics { get; set; }

        public TradeStatistics? Statistics
        {
            get => _statistics;
            set
            {
                _statistics = value;
                OnPropertyChanged();
                NotifyStatisticsChanged();
            }
        }  

        public string FloorDisplay =>
            Statistics == null
                ? "-"
                : $"{Statistics.Percentiles.Floor:0.##} Ist";

        public string TypicalDisplay =>
            Statistics == null
                ? "-"
                : $"{Statistics.Percentiles.Typical:0.##} Ist";

        public string GoodDisplay =>
            Statistics == null
                ? "-"
                : $"{Statistics.Percentiles.Good:0.##} Ist";      

        public string HighDisplay =>
            Statistics == null
                ? "-"
                : $"{Statistics.Percentiles.High:0.##} Ist";
        public string FloorEstimateDisplay =>
          Statistics == null
              ? "-"
              : $"~{ GetRuneHint(Statistics.Percentiles.Floor)}";

        public string TypicalEstimateDisplay =>
         Statistics == null
             ? "-"
             : $"~{GetRuneHint(Statistics.Percentiles.Typical)}";

        public string GoodEstimateDisplay =>
      Statistics == null
          ? "-"
          : $"~{GetRuneHint(Statistics.Percentiles.Good)}";


        public string HighEstimateDisplay =>
           Statistics == null
               ? "-"
               : $"~{GetRuneHint(Statistics.Percentiles.High)}";  


        public bool HasStatistics => Statistics != null;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void NotifyStatisticsChanged()
        {
            OnPropertyChanged(nameof(HasStatistics));
            OnPropertyChanged(nameof(FloorDisplay));
            OnPropertyChanged(nameof(TypicalDisplay));
            OnPropertyChanged(nameof(GoodDisplay));
            OnPropertyChanged(nameof(HighDisplay));
            OnPropertyChanged(nameof(FloorEstimateDisplay));
            OnPropertyChanged(nameof(TypicalEstimateDisplay));
            OnPropertyChanged(nameof(GoodEstimateDisplay));
            OnPropertyChanged(nameof(HighEstimateDisplay));
        }

        private string GetRuneHint(double value)
        {
            if (Statistics?.RuneValues == null) return "";

            var closest = Statistics.RuneValues
                .OrderBy(r => Math.Abs(r.IstValue - value))
                .First();

            return closest.ShortName;
        }
    }
}
