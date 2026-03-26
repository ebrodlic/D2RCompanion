using D2RPriceChecker.Pipelines;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace D2RPriceChecker.Services
{
    public class DatasetManager
    {
        private readonly string _basePath;

        private readonly string _screenshotsDirName = "Screenshots";
        private readonly string _masksDirName = "Masks";
        private readonly string _tooltipsDirName = "Tooltips";
        private readonly string _linesDirName = "Lines";

        private readonly bool _saveScreenshots = true;
        private readonly bool _saveMasks = false;
        private readonly bool _saveTooltips = true;
        private readonly bool _saveLines = true;


        public DatasetManager()
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _basePath = Path.Combine(root, "D2RPriceChecker", "Cache");

            Directory.CreateDirectory(Path.Combine(_basePath, _screenshotsDirName));
            Directory.CreateDirectory(Path.Combine(_basePath, _masksDirName));
            Directory.CreateDirectory(Path.Combine(_basePath, _tooltipsDirName));
            Directory.CreateDirectory(Path.Combine(_basePath, _linesDirName));
        }

        public void Save(string id, TooltipDetectionPipelineResult result)
        {
            var screenshotPath = Path.Combine(_basePath, _screenshotsDirName, $"{id}.png");
            var tooltipPath = Path.Combine(_basePath, _tooltipsDirName, $"{id}.png");
            var maskPath = Path.Combine(_basePath, _masksDirName, $"{id}.png");

            if (_saveScreenshots)
                result.Screenshot.Save(screenshotPath, ImageFormat.Png);

            if(_saveMasks)
                result.BorderMask?.Save(maskPath, ImageFormat.Png);

            if (_saveTooltips)
                result.Tooltip?.Save(tooltipPath, ImageFormat.Png);
        }

        public void Save(string id, TooltipLineSegmetnationPipelineResult result)
        {
            var linesPath = Path.Combine(_basePath, _linesDirName);

            if (_saveLines)
            {
                for (int i = 0; i < result.TooltipLines.Count; i++)
                {
                    var line = result.TooltipLines[i];
                    var linePath = Path.Combine(linesPath, $"{id}_{i:D2}.png");

                    line.Save(linePath, ImageFormat.Png);
                }
            }
        }
    }
}
