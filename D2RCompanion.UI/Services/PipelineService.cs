// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using D2RCompanion.Core.Items;
using D2RCompanion.Core.Pipelines;
using D2RCompanion.Pipelines;
using D2RCompanion.Services;
using D2RCompanion.UI;
using D2RCompanion.UI.Traderie;
using D2RCompanion.UI.Util;
using D2RCompanion.UI.Views;
using Microsoft.Extensions.Logging;

namespace D2RCompanion.UI.Services
{
    public class PipelineService
    {
        private readonly ScreenshotService _screenshots;
        private readonly OcrService _ocr;
        private readonly IItemBaseNameProvider _items;

        private readonly CacheService _cache;
        private readonly SettingsService _settings;

        private readonly ILogger _logger;

        public PipelineService(
            ScreenshotService screenshots,
            OcrService ocr,
            IItemBaseNameProvider provider,
            CacheService cache,
            ILogger<PipelineService> logger
            )
        {
            _screenshots = screenshots;
            _ocr = ocr;
            _items = provider;
            _cache = cache;

            _logger = logger;
        }

        public async Task<PipelineResult> RunAsync()
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            var screenshot = _screenshots.CaptureGameWindow("D2R");

            var detection = new TooltipDetectionPipeline().Run(screenshot);
           
            _cache.Save(timestamp, detection);

            if (!detection.IsTooltipFound())
            {
                _logger.LogInformation("Tooltip not detected, fallback on yolo");

                detection.Tooltip =
                   new TooltipDetectionPipelineYolo("Models/d2r_tooltip_yolo_best.onnx")
                   .Run(screenshot);
            }

            var segmentation =
                new TooltipLineSegmentationPipeline()
                .Run(detection.Tooltip, new TooltipLineSegmentationPipelineSettings());

            _cache.Save(timestamp, segmentation);

            var text = await Task.Run(() =>
                _ocr.PredictTextBatch(segmentation.TooltipLines));

            var item = new ItemAnalysisPipeline(_items)
                .Run(text, segmentation);

            return new PipelineResult
            {
                ItemText = text,
                ItemData = item
            };
        }
    }

    public class PipelineResult
    {
        public IReadOnlyList<string> ItemText { get; init; }
        public Item ItemData { get; init; }
    }
}
//private TooltipLineSegmentationPipelineResult RunSegmentationPipeline(string timestamp, Bitmap tooltip)
//{
//    Stopwatch stopwatch = new Stopwatch();

//    // Start measuring time
//    stopwatch.Start();

//    var settings = new TooltipLineSegmentationPipelineSettings();
//    var segmentationResult = new TooltipLineSegmentationPipeline().Run(tooltip, settings);

//    // Stop measuring time
//    stopwatch.Stop();

//    // Print the elapsed time in milliseconds
//    Console.WriteLine($"Tooltip segmentation took {stopwatch.ElapsedMilliseconds} ms");

//    //TODO if save set in settings?
//    SavePipelineResultData(timestamp, segmentationResult);

//    return segmentationResult;
//}


