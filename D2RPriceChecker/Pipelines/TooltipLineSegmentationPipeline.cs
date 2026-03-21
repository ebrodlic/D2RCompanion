using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.Pipelines;
using System.Text;
using System.Windows.Controls;

namespace D2RPriceChecker.Pipelines
{
    internal class TooltipLineSegmentationPipeline
    {
        private TooltipLineSegmentationPipelineSettings Settings { get; set; }


        public TooltipLineSegmetnationPipelineResult Run(Bitmap tooltip, TooltipLineSegmentationPipelineSettings settings)
        {
            Settings = settings;

            var result = new TooltipLineSegmetnationPipelineResult(tooltip);

            //var rowsMask = GetRowsMask(tooltip);

            var blobs = FindTextBlobs(tooltip);

            var lines = GetLinesFromBlobDetections(tooltip, blobs);

            result.TooltipLines = lines;

            return result;
        }

        private List<Bitmap> GetLinesFromBlobDetections(Bitmap tooltip, List<ContentBlobDetection> blobs)
        {
            var lines = new List<Bitmap>();

            foreach (var blob in blobs)
            {
                // Add capitalization effect to line start
                int yCapitalizationStart = blob.StartY - Settings.CapitalizationOffset;
                int yfloorOffset = blob.EndY + Settings.FloorOffset;

                // Add padding to the line boundaries
                int paddedStartY = Math.Max(0, yCapitalizationStart - Settings.PaddingTop);
                int paddedEndY = Math.Min(tooltip.Height - 1, yfloorOffset + Settings.PaddingBottom);
                int paddedLineHeight = paddedEndY - paddedStartY + 1;

                int paddedStartX = Math.Max(0, blob.StartX - 25); // TODO - add padding left/right settings if needed DOCUMENT: padding left needs to be higher for runeword and capitalization width
                int paddedEndX = Math.Min(tooltip.Width - 1, blob.EndX + 25);
                int paddedLineWidth = paddedEndX - paddedStartX + 1;

                // Extract the line bitmap with padding
                Bitmap lineBitmap = tooltip.Clone(new Rectangle(paddedStartX, paddedStartY, Math.Max(paddedLineWidth, 1), Math.Max(paddedLineHeight, 1)), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                lines.Add(lineBitmap);
            }
            return lines;
        }

        // Scan tooltip rows to detect areas of text body (10-11 min px height mid section)
        private List<ContentBlobDetection> FindTextBlobs(Bitmap tooltip)
        {
            var result = new List<ContentBlobDetection>();

            //Waiting on 10 vertical matches
            var sequenceCounter = 0;
            var sequenceStartY = -1;
            var sequenceEndY = -1;

            int mid = tooltip.Width / 2;

            // Scan the rows from top to bottom
            for (int y = 0; y < tooltip.Height; y++)
            {
                var scanResult = ScanRowLeft(tooltip, y);

                if(scanResult.ContentPixelCount > Settings.ContentCutoffValue)
                {
                    scanResult.HasContent = true;
                    //scanResult.MaxX = mid + (mid - scanResult.MinX);
                }
                else
                {
                    scanResult = ScanRowRight(tooltip, y, scanResult.ContentPixelCount);

                    if (scanResult.ContentPixelCount > Settings.ContentCutoffValue)
                    {
                        scanResult.HasContent = true;
                        //scanResult.MinX = mid - (scanResult.MaxX - mid);
                    }
                }

                if (scanResult.HasContent)
                {
                    if (sequenceStartY == -1)
                        sequenceStartY = y; // mark the start of a sequence

                    sequenceCounter++;
                }
                else
                {
                    if (sequenceCounter > 0)
                    {
                        if (sequenceCounter >= 10)
                        {
                            sequenceEndY = y - 1; // end of the sequence is the last true index

                            result.Add(new ContentBlobDetection
                            {
                                StartY = sequenceStartY,
                                EndY = sequenceEndY,
                                StartX = 0,
                                EndX = tooltip.Width
                            });
                        }

                        sequenceCounter = 0;
                        sequenceStartY = -1;
                        sequenceEndY = -1;
                    }
                }
            }

            return result;
        }

        private RowScanResult ScanRowLeft(Bitmap tooltip, int y)
        {
            var result = new RowScanResult();

            int mid = tooltip.Width / 2;
            int distanceCounter = 0;

            for (int x = mid; x > 0; x--)
            {
                if (distanceCounter >= Settings.DistanceThreshold)
                {
                    break; // stop scanning this row if we have reached the distance threshold without matches
                }

                Color pixel = tooltip.GetPixel(x, y);

                if (IsPixelContent(pixel))
                {
                    result.ContentPixelCount++;
                    result.MinX = x;

                    distanceCounter = 0; // reset distance counter on match
                }
                else
                {
                    distanceCounter++;
                }
            }

            return result;
        }

        private RowScanResult ScanRowRight(Bitmap tooltip, int y, int inflateCount = 0)
        {
            var result = new RowScanResult();

            // For letters like spirit, with very low density, we want to include findings from the left side
            if(inflateCount > 0)
                result.ContentPixelCount = inflateCount;

            int mid = tooltip.Width / 2;
            int distanceCounter = 0;

            for (int x = mid; x < tooltip.Width; x++)
            {
                if (distanceCounter >= Settings.DistanceThreshold)
                {
                    break; // stop scanning this row if we have reached the distance threshold without matches
                }

                Color pixel = tooltip.GetPixel(x, y);

                if (IsPixelContent(pixel))
                {
                    result.ContentPixelCount++;
                    result.MaxX = x;

                    distanceCounter = 0; // reset distance counter on match
                }
                else
                {
                    distanceCounter++;
                }
            }

            return result;
        }


        //private RowScanResult[] GetRowsMask(Bitmap tooltip, bool smootheResults = true)
        //{
        //    var rowsMask = new RowScanResult[tooltip.Height];

        //    int mid = tooltip.Width / 2;

        //    for (int y = 0; y < tooltip.Height; y++)
        //    {
        //        rowsMask[y] = new RowScanResult();

        //        var countDistance = 0;

        //        for (int x = mid; x > 0; x--)
        //        {
        //            if (countDistance >= Settings.DistanceThreshold)
        //            {
        //                break; // stop scanning this row if we have reached the distance threshold without matches
        //            }


        //            Color pixel = tooltip.GetPixel(x, y);

        //            if (IsPixelContent(pixel))
        //            {
        //                rowsMask[y].ContentPixelCount++;
        //                rowsMask[y].MinX = x;

        //                countDistance = 0; // reset distance counter on match
        //            }
        //            else
        //            {
        //                countDistance++;
        //            }
        //        }

        //        if (rowsMask[y].ContentPixelCount > Settings.ContentCutoffValue)
        //        {
        //            rowsMask[y].HasContent = true;
        //            rowsMask[y].MaxX = mid + (mid - rowsMask[y].MinX);

        //            //TODO if we need more check of detection ratio
        //            //if (rowsMask[y].ContentPixelCount / rowsMask[y].PixelsChecked > 0.001)
        //            //{

        //            //}
        //        }

        //        //if we found enough pixels for total checked threshold, mark the row as content, update the max value for x
        //    }

        //    if (smootheResults)
        //        FillShortGaps(rowsMask, Settings.LeadingThreshold);

        //    return rowsMask;
        //}

        // Since actual tooltip text content is split into lines, we know we cannot have a gap smaller than the 2 paddings we use. 
        // Fills short gaps of false values in the rowsMask with true, based on the specified minimum gap size.
        // Smoothes out the result increasing confidence that we are not missing any lines due to small gaps in the content rows.
        //private void FillShortGaps(RowScanResult[] rowsMask, int minGapSize)
        //{
        //    int startIdx = -1;  // start of a zero sequence

        //    for (int i = 0; i < rowsMask.Length; i++)
        //    {
        //        if (!rowsMask[i].HasContent)
        //        {
        //            if (startIdx == -1)
        //                startIdx = i;  // mark the start of a zero sequence
        //        }
        //        else
        //        {
        //            if (startIdx != -1)
        //            {
        //                int gapSize = i - startIdx;
        //                if (gapSize < minGapSize)
        //                {
        //                    // Fill the gap with true values
        //                    for (int j = startIdx; j < i; j++)
        //                        rowsMask[j].HasContent = true;
        //                }
        //                startIdx = -1;  // reset for the next sequence
        //            }
        //        }
        //    }
        //    //handle trailing zeroes at the end?
        //}

        private bool IsPixelContent(Color pixel)
        {
            int r = pixel.R;
            int g = pixel.G;
            int b = pixel.B;

            float brightness = (0.299f * pixel.R) + (0.587f * pixel.G) + (0.114f * pixel.B);
            int maxChannel = Math.Max(pixel.R, Math.Max(pixel.G, pixel.B));

            var isContent = brightness > Settings.BrightnessThreshold || maxChannel > Settings.MaxChannelThreshold;

            return isContent;
        }

        //private bool IsPixelTextColor(Color pixel)
        //{
        //    bool isYellow = hue >= 40 && hue <= 65 && saturation > 0.4 && value > 0.5;
        //    bool isBlue = hue >= 180 && hue <= 250 && saturation > 0.4;
        //    bool isGreen = hue >= 80 && hue <= 160 && saturation > 0.4;
        //}

        private List<Bitmap> GetLinesSimple(Bitmap tooltip, RowScanResult[] rowsMask)
        {
            var lines = new List<Bitmap>();

            var yIndexStart = -1;
            var yIndexEnd = -1;
            var xIndexStart = -1;
            var xIndexEnd = -1;

            for (int y = 0; y < rowsMask.Length; y++)
            {
                // Found content row, find y block and boundaries for x min and max
                if (rowsMask[y].HasContent)
                {
                    if (yIndexStart == -1)
                    {
                        yIndexStart = y;  // mark the start of a line sequence  

                        xIndexStart = rowsMask[y].MinX;
                        xIndexEnd = rowsMask[y].MaxX;
                    }
                    else
                    {
                        xIndexStart = Math.Min(xIndexStart, rowsMask[y].MinX);
                        xIndexEnd = Math.Max(xIndexEnd, rowsMask[y].MaxX);
                    }
                }
                else
                {
                    // End of a y block, extract the line bitmap if we had a valid start
                    if (yIndexStart != -1)
                    {
                        yIndexEnd = y - 1;  // end of the line is the last true index                    

                        // Add capitalization effect to line start
                        int yCapitalizationStart = yIndexStart - Settings.CapitalizationOffset;
                        int yfloorOffset = yIndexEnd + Settings.FloorOffset;

                        // Add padding to the line boundaries
                        int paddedStartY = Math.Max(0, yCapitalizationStart - Settings.PaddingTop);
                        int paddedEndY = Math.Min(tooltip.Height - 1, yfloorOffset + Settings.PaddingBottom);
                        int paddedLineHeight = paddedEndY - paddedStartY + 1;

                        int paddedStartX = Math.Max(0, xIndexStart - 25); // TODO - add padding left/right settings if needed DOCUMENT: padding left needs to be higher for runeword and capitalization width
                        int paddedEndX = Math.Min(tooltip.Width - 1, xIndexEnd + 25);
                        int paddedLineWidth = paddedEndX - paddedStartX + 1;

                        // Extract the line bitmap with padding
                        Bitmap lineBitmap = tooltip.Clone(new Rectangle(paddedStartX, paddedStartY, Math.Max(paddedLineWidth, 1), Math.Max(paddedLineHeight, 1)), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        lines.Add(lineBitmap);

                        // reset for the next line

                        yIndexStart = -1;
                        yIndexEnd = -1;
                        xIndexStart = -1;
                        xIndexEnd = -1;

                        y = paddedEndY;  // Skip to the end of this line to avoid overlapping
                    }
                }
            }

            return lines;
        }
    }

    public class RowScanResult
    {
        public bool HasContent { get; set; }
        public int ContentPixelCount { get; set; }
        public int MinX { get; set; }
        public int MaxX { get; set; }
    }

    public class ContentBlobDetection
    {
        public int StartY { get; set; }
        public int EndY { get; set; }
        public int StartX { get; set; }
        public int EndX { get; set; }
    }
}

