// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using D2RCompanion.Core.Items;
using D2RCompanion.Core.Pricing;
using D2RCompanion.Core.Traderie.Domain;
using D2RCompanion.UI.Messages;
using D2RCompanion.UI.Services;
using D2RCompanion.UI.Traderie;
using Microsoft.Extensions.Logging;

namespace D2RCompanion.UI.ViewModels
{
    public partial class PriceCheckViewModel : ObservableObject
    {
        // DESCRIPTION
        [ObservableProperty]
        public ObservableCollection<string> ocrText = new ObservableCollection<string>();

        [ObservableProperty]
        public Item itemMetadata = new Item();

        [ObservableProperty]
        public ObservableCollection<Trade> trades = new ObservableCollection<Trade>();

        [ObservableProperty]
        public TradeActivityInfo activity = new TradeActivityInfo();

        public ObservableCollection<RuneValue> RuneValuesDisplay =>
            Statistics?.RuneValues != null
                ? new ObservableCollection<RuneValue>(Statistics.RuneValues)
                : new ObservableCollection<RuneValue>();

        [ObservableProperty]
        public TradeStatistics statistics;

        [ObservableProperty]
        public double pricePrediction  = 0;

        [ObservableProperty]
        public string pricePredictionHint = "";

        [ObservableProperty]
        public double pricePredictionConfidence = 0;

        [ObservableProperty]
        private bool isRuneInfoVisible;

        [ObservableProperty]
        private bool isRuneInfoPinned;

        // TODO mb also update these live
        public Brush ActivityBorderBrush { get; set; }
        public Brush ActivityTextBrush { get; set; }

        private readonly TraderieClient _traderieClient;
        private readonly ILogger _logger;

        public PriceCheckViewModel(
            TraderieClient traderie,
            ILogger<PriceCheckViewModel>logger
            )
        {
            _traderieClient = traderie;
            _logger = logger;

            RegisterMessageHandlers();
        }

        private void RegisterMessageHandlers()
        {
            WeakReferenceMessenger.Default.Register<PipelineCompletedMessage>(this, (r, m) =>
            {
                // Show the Overlay
                WeakReferenceMessenger.Default.Send(
                    new OverlayVisibilityRequestMessage(true)
                );

                // Clear existing data
                ClearData();

                // Load new data
                LoadPipelineData(m.Result);

                // Fetch price/trade data from Traderie
                _ = FetchPriceData();
                _ = FetchStatistics();


            });
        }

        public void LoadPipelineData(PipelineResult result)
        {
            //TODO filter here, get rid of requirements lines, show only attributes
            foreach (var item in result.ItemText)
            {
                OcrText.Add(item);
            }

            ItemMetadata = result.ItemData;
        }

        private async Task FetchPriceData()
        {
            var trades = await _traderieClient.GetTradesDataAsync(ItemMetadata);

            foreach (var trade in trades)
            {
                Trades.Add(trade);
            }

            var calculator = new TradeActivityCalculator();

            var info = calculator.Calculate(trades);

            Activity = info;

            //TODO get rid of this
            NotifyActivityChanged();

          
        }
        private async Task FetchStatistics()
        {
            var stats = await _traderieClient.GetPriceStatisticsAsync(ItemMetadata);

            Statistics = stats;

            // TODO - get rid of this
            OnPropertyChanged(nameof(Statistics)); // Notify that Statistics changed
            OnPropertyChanged(nameof(FloorDisplay));  // Notify that dependent properties need recalculation
            OnPropertyChanged(nameof(TypicalDisplay));
            OnPropertyChanged(nameof(GoodDisplay));
            OnPropertyChanged(nameof(HighDisplay));
            OnPropertyChanged(nameof(FloorEstimateDisplay));
            OnPropertyChanged(nameof(TypicalEstimateDisplay));
            OnPropertyChanged(nameof(GoodEstimateDisplay));
            OnPropertyChanged(nameof(HighEstimateDisplay));

            // TODO : extract this
            RefreshPricePrediction();
        }

        public void ClearData()
        {
            // 1. Collections
            ocrText.Clear();
            Trades.Clear();

            // 2. Domain objects
            Statistics = null;
            Activity = new TradeActivityInfo();

            // 3. Prediction state
            PricePrediction = 0;
            PricePredictionHint = "";
            PricePredictionConfidence = 0;

            // 4. Rune UI state
            IsRuneInfoVisible = false;
            IsRuneInfoPinned = false;

            // 5. Brushes (important: reset visuals too)
            ActivityBorderBrush = new SolidColorBrush(Color.FromRgb(90, 90, 90));
            ActivityTextBrush = new SolidColorBrush(Lighten(Color.FromRgb(90, 90, 90)));

            ResetBrushes();

            // 7. Derived displays
            //NotifyStatisticsChanged();
        }

        [RelayCommand]
        public void Close()
        {
            WeakReferenceMessenger.Default.Send(
                new NavigationRequestMessage(OverlayContentView.Home));

            //close overlay
            WeakReferenceMessenger.Default.Send(
                new OverlayVisibilityRequestMessage(false)
            );

            ClearData();

            //WeakReferenceMessenger.Default.Send(
            //    new NavigationRequestMessage
            //    {
            //        FromView = OverlayContentView.Settings,
            //        ToView = OverlayContentView.Home,
            //    }
            //);


        }


        //public ObservableCollection<OcrLine> OcrLines { get; set; } = new();

       



        public string PriceGroupsDisplay =>
            Trades.FirstOrDefault()?.PriceGroups == null
                ? "-"
                : string.Join(" OR ",
                    Trades.First().PriceGroups.Select(g =>
                        string.Join(" + ",
                            g.Prices.Select(p => $"{p.Quantity}x {p.Name}")
                        )
                    ));

    


        [RelayCommand]
        private void ToggleRuneInfo()
        {
            IsRuneInfoVisible = !IsRuneInfoVisible;
        }

        [RelayCommand]
        private void ToggleRuneInfoPinned()
        {
            IsRuneInfoPinned = !IsRuneInfoPinned;

            // If pinned ON → force visible
            // If pinned OFF → revert to hover-only behavior
            IsRuneInfoVisible = IsRuneInfoPinned;
        }

        public void RuneInfoHoverEnter()
        {
            if (!IsRuneInfoPinned)
                IsRuneInfoVisible = true;
        }

        public void RuneInfoHoverLeave()
        {
            if (!IsRuneInfoPinned)
                IsRuneInfoVisible = false;
        }

        private void ResetBrushes()
        {
            ActivityBorderBrush = new SolidColorBrush(Color.FromRgb(90, 90, 90));
            ActivityTextBrush = new SolidColorBrush(Lighten(Color.FromRgb(90, 90, 90)));
            OnPropertyChanged(nameof(ActivityBorderBrush));
            OnPropertyChanged(nameof(ActivityTextBrush));
        }


        public void RecalculateActivity()
        {
            var calc = new TradeActivityCalculator();

            Activity = calc.Calculate(Trades.ToList());

            NotifyActivityChanged();

            OnPropertyChanged(nameof(Activity));
            OnPropertyChanged(nameof(ActivityBorderBrush));
            OnPropertyChanged(nameof(ActivityTextBrush));
        }

        private void NotifyActivityChanged()
        {
            if (Activity == null)
                return;

            Color baseColor = Activity.Level switch
            {
                ActivityLevel.Dead => Color.FromRgb(90, 90, 90),     // gray
                ActivityLevel.Low => Color.FromRgb(176, 0, 32),     // red
                ActivityLevel.Medium => Color.FromRgb(230, 126, 34),   // orange
                ActivityLevel.High => Color.FromRgb(241, 196, 15),   // yellow
                ActivityLevel.VeryHigh => Color.FromRgb(46, 204, 113),   // green
                _ => Color.FromRgb(90, 90, 90)
            };

            ActivityBorderBrush = new SolidColorBrush(baseColor);
            ActivityTextBrush = new SolidColorBrush(Lighten(baseColor, 0.5));

            OnPropertyChanged(nameof(Activity));
            OnPropertyChanged(nameof(ActivityBorderBrush));
            OnPropertyChanged(nameof(ActivityTextBrush));
        }

        private Color Lighten(Color color, double factor = 0.5)
        {
            return Color.FromRgb(
                (byte)(color.R + (255 - color.R) * factor),
                (byte)(color.G + (255 - color.G) * factor),
                (byte)(color.B + (255 - color.B) * factor)
            );
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
              : $"~{GetRuneHint(Statistics.Percentiles.Floor)}";

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


        public void RefreshPricePrediction()
        {
            var table = new RuneValueTable(Statistics.RuneValues);
            var prediction = new PricePredictionService(table);

            var lines = ocrText.ToList();

            var predictionResult = prediction.Predict(lines, Trades.ToList());

            PricePrediction = predictionResult.Value;
            PricePredictionHint = "~" + GetRuneHint(predictionResult.Value) + " ";
            PricePredictionConfidence = predictionResult.Confidence;

            OnPropertyChanged(nameof(PricePrediction));
            OnPropertyChanged(nameof(PricePredictionHint));
            OnPropertyChanged(nameof(PricePredictionConfidence));



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

    public class PriceGroupDisplay
    {
        public string Text { get; set; } = "";
        public bool IsOr { get; set; }
    }
}
