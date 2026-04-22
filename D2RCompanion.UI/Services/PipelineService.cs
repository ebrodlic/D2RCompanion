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
using D2RCompanion.UI.Views;

namespace D2RCompanion.UI.Services
{
    public class PipelineService
    {
        private readonly ScreenshotService _screenshots;
        private readonly OcrService _ocr;
        private readonly IItemBaseNameProvider _items;

        public PipelineService(
            ScreenshotService screenshots,
            OcrService ocr,
            IItemBaseNameProvider items)
        {
            _screenshots = screenshots;
            _ocr = ocr;
            _items = items;
        }

        public async Task RunAsync()
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            var screenshot = _screenshots.CaptureGameWindow("D2R");

            var detection = new TooltipDetectionPipeline().Run(screenshot);

            if (!detection.IsTooltipFound())
                detection.Tooltip =
                    new TooltipDetectionPipelineYolo("Models/d2r_tooltip_yolo_best.onnx")
                    .Run(screenshot);

            var segmentation =
                new TooltipLineSegmentationPipeline()
                .Run(detection.Tooltip!, new TooltipLineSegmentationPipelineSettings());

            var text = await Task.Run(() =>
                _ocr.PredictTextBatch(segmentation.TooltipLines));

            var item = new ItemAnalysisPipeline(_items)
                .Run(text, segmentation);
        }
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

//private void SavePipelineResultData(string timestamp, TooltipDetectionPipelineResult result)
//{
//    var datasetManager = ((App)System.Windows.Application.Current).Cache;

//    datasetManager.Save(timestamp, result);
//}
//private void SavePipelineResultData(string timestamp, TooltipLineSegmentationPipelineResult result)
//{
//    var datasetManager = ((App)System.Windows.Application.Current).Cache;

//    datasetManager.Save(timestamp, result);
//}
