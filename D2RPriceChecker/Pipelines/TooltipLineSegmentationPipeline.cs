using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Pipelines;
using System.Text;
using System.Windows.Controls;

namespace D2RPriceChecker.Pipelines
{
    internal class TooltipLineSegmentationPipeline
    {
        public int _brightnessThreshold { get; set; } = 80;
        public int _maxChannelThreshold { get; set; } = 140;
        public double _densityThresholdRatio { get; set; } = 0.005;
        public int _padding { get; set; } = 6;

        // Row Density Calculation/Brightness thresholding
        // - Iterate through each row of the tooltip image and calculate the average brightness.    

        // go through each row of the tooltip image and calculate the average brightness
        // if the average brightness is above a certain threshold, consider it as a line break
        // store the positive lines in a list, until we hit empty line
        public TooltipLineSegmetnationPipelineResult Run(Bitmap tooltip)
        {
            var result = new TooltipLineSegmetnationPipelineResult(tooltip);

            var rowsMask = GetRowsContentMask(tooltip);
            var lines = GetLinesBitmaps(tooltip, rowsMask, _padding);

            result.TooltipLines = lines;

            return result;
        }

        private List<Bitmap> GetLinesBitmaps(Bitmap tooltip, bool[] rowsMask, int padding)
        {
            var lines = new List<Bitmap>();

            var xIndexStart = -1;
            var xIndexEnd = -1;

            var yIndexStart = -1;
            var yIndexEnd = -1;

            for (int y = 0; y < rowsMask.Length; y++)
            {
                // Found content row, find y block and boundaries for x min and max
                if (rowsMask[y])
                {
                    if (yIndexStart == -1)
                        yIndexStart = y;  // mark the start of a line sequence

                    for (int x = 0; x < tooltip.Width; x++)
                    {
                        Color pixel = tooltip.GetPixel(x, y);

                        if (IsPixelContent(pixel))
                        {
                            // mark the start of a line sequence
                            if (xIndexStart == -1)
                            {
                                xIndexStart = x;
                                xIndexEnd = x;
                            }
                            else
                            {
                                xIndexStart = Math.Min(xIndexStart, x);
                                xIndexEnd = Math.Max(xIndexEnd, x);
                            } 
                        }
                    }
                }
                else
                {
                    // End of a y block, extract the line bitmap if we had a valid start
                    if (yIndexStart != -1)
                    {
                        yIndexEnd = y - 1;  // end of the line is the last true index                    

                        // Add padding to the line boundaries
                        int paddedStartY = Math.Max(0, yIndexStart - padding);
                        int paddedEndY = Math.Min(tooltip.Height - 1, yIndexEnd + padding);
                        int paddedLineHeight = paddedEndY - paddedStartY + 1;

                        int paddedStartX = Math.Max(0, xIndexStart - padding);
                        int paddedEndX = Math.Min(tooltip.Width - 1, xIndexEnd + padding);
                        int paddedLineWidth = paddedEndX - paddedStartX + 1;

                        // Extract the line bitmap with padding
                        Bitmap lineBitmap = tooltip.Clone(new Rectangle(paddedStartX, paddedStartY, paddedLineWidth, paddedLineHeight), tooltip.PixelFormat);
                        lines.Add(lineBitmap);

                        // reset for the next line
                        xIndexStart = -1;
                        xIndexEnd = -1;
                        yIndexStart = -1;
                        yIndexEnd = -1;

                        y = paddedEndY;  // Skip to the end of this line to avoid overlapping
                    }
                }
            }

            return lines;
        }

        private bool[] GetRowsContentMask(Bitmap tooltip, bool smootheResults = true) 
        {
            var minDensity = tooltip.Width * _densityThresholdRatio;

            bool[] rowsMap = new bool[tooltip.Height];

            for (int y = 0; y < tooltip.Height; y++)
            {
                var count = 0;

                for (int x = 0; x < tooltip.Width; x++)
                {
                    Color pixel = tooltip.GetPixel(x, y);

                    if (IsPixelContent(pixel))
                        count++;
                }

                rowsMap[y] = count > minDensity ? true : false;
            }

            if(smootheResults)  
                FillShortGaps(rowsMap, _padding);
           
            return rowsMap;
        }

        private bool IsPixelContent(Color pixel)
        {
            int r = pixel.R;
            int g = pixel.G;
            int b = pixel.B;

            float brightness = (0.299f * pixel.R) + (0.587f * pixel.G) + (0.114f * pixel.B);
            int maxChannel = Math.Max(pixel.R, Math.Max(pixel.G, pixel.B));

            return brightness > _brightnessThreshold || maxChannel > _maxChannelThreshold;
        }

        private void FillShortGaps(bool[] rowsMask, int minGapSize)
        {
            int startIdx = -1;  // start of a zero sequence

            for (int i = 0; i < rowsMask.Length; i++)
            {
                if (!rowsMask[i])
                {
                    if (startIdx == -1)
                        startIdx = i;  // mark the start of a zero sequence
                }
                else
                {
                    if (startIdx != -1)
                    {
                        int gapSize = i - startIdx;
                        if (gapSize < minGapSize)
                        {
                            // Fill the gap with true values
                            for (int j = startIdx; j < i; j++)
                                rowsMask[j] = true;
                        }
                        startIdx = -1;  // reset for the next sequence
                    }
                }
            }
            //handle trailing zeroes at the end?
        }
    }
}
