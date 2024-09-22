using CutCraftEngineData.DataInput;
using CutGLib;
using NLog.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CutCraftEngineWebSocketCGLService.CGLCalculator
{
    public class CGLStatistics
    {
        private readonly CutEngine _cutEngine;

        private int _stockItemId;  // The stock item ID to get statistics.

        private int _usedSheet; // Number of sheets used.

        private double _field;  // The total surface area of all used stock items.
        private double _usedField;  // The total surface area of all used pieces.
        private double _wasteField;  // The total waste area.
        private double? _unusedField = null;  // The total surface area of usable waste. (CutGLib unavailable)


        private int _cutTrimCount;  // The number of trim cuts.
        private double _cutsTrimLength;  // The total length of all trim cuts

        private int _cutCount;  // The number of cuts.
        private double _cutsLength;  // The total length of all the cuts

        public double Field => _field;
        public double UsedField => _usedField;
        public double WasteField => _wasteField;
        public double? UnusedField => _unusedField;

        public CGLStatistics(CutEngineInitializer calculator, int stockItemId)
        {
            if (calculator.IsExecuted == false)
                throw new InvalidOperationException("The calculator must be executed before reading the statistics.");

            _cutEngine = calculator.GetCutEngine();
            
            calculateFields(out _field, out _usedField, out _wasteField, out _usedSheet);
        }

        private void getCountLength(out int cutTrimCount, out double cutsTrimLength, out int cutCount, out double cutsLength)
        {
            cutTrimCount = 0;
            cutsTrimLength = 0d;
            cutCount = 0;
            cutsLength = 0d;


        }

        private void calculateCount(out int cutCount, out double cutsLength)
        {
            int StockNo, iCut, iPart;
            long CutsCount;
            double Width, Height, X1 = 0, Y1 = 0, X2 = 0, Y2 = 0;
            bool active;
            string id;

            cutCount = 0;
            cutsLength = 0d;

            // Output guilltoine cuts for each sheet
            for (StockNo = 0; StockNo < _cutEngine.StockCount; StockNo++)
            {
                _cutEngine.GetStockInfo(StockNo, out Width, out Height, out active);
                // Sheet was not used during calculation
                if (!active)
                {
                    Console.WriteLine("Sheet={0} was not used.", StockNo);
                    continue;
                }
                Console.WriteLine("Sheet={0}: Width={1} Height={2}", StockNo, Width, Height);
                // First output any trim cuts for the sheet StockNo
                CutsCount = _cutEngine.GetStockTrimCutCount(StockNo);
                for (iCut = 0; iCut < CutsCount; iCut++)
                {
                    _cutEngine.GetStockTrimCut(StockNo, iCut, out X1, out Y1, out X2, out Y2);
                    Console.WriteLine("Trim  Cut={0}:  X1={1};  Y1={2};  X2={3};  Y2={4}", iCut, X1, Y1, X2, Y2);
                }
                // Now output any actual cuts for the sheet StockNo
                CutsCount = _cutEngine.GetStockCutCount(StockNo);
                for (iCut = 0; iCut < CutsCount; iCut++)
                {
                    _cutEngine.GetStockCut(StockNo, iCut, out X1, out Y1, out X2, out Y2);
                    Console.WriteLine("Cut={0}:  X1={1};  Y1={2};  X2={3};  Y2={4}", iCut, X1, Y1, X2, Y2);
                }
            }
            Console.WriteLine();

            // Get parts locations
            double W = 0, H = 0, X = 0, Y = 0;
            bool Rotated;
            Console.WriteLine("======================================");
            Console.WriteLine("OUTPUT PARTS RESULTS");
            Console.WriteLine("======================================");

            Console.WriteLine("Part Count={0}", _cutEngine.PartCount);
            for (iPart = 0; iPart < _cutEngine.PartCount; iPart++)
            {
                // Get sizes and location of the source part with index Iter
                // in case of incomplete optimization the part can be unplaced
                // and the function returns FALSE.
                if (_cutEngine.GetResultPart(iPart, out StockNo, out W, out H, out X, out Y, out Rotated, out id))
                {
                    Console.WriteLine("Part ID={0};  sheet={1};  X={2};  Y={3};  Width={4};  Height={5}",
                                  id, StockNo, X, Y, W, H);
                }
                else Console.WriteLine("Part {0} was not placed", iPart);
            }
            Console.WriteLine();
        }

        /// <summary>
        /// </summary>
        private void calculateFields(out double field, out double usedField, out double wasteField, out int usedSheet)
        {

            int sheetIndex, StockCount, iPart, iLayout, partCount, partIndex, tmp, iSheet;
            double width, height, X, Y, W, H;
            bool rotated, sheetActive;

            field = 0d;
            usedField = 0d;
            wasteField = 0d;
            usedSheet = _cutEngine.UsedStockCount;

            for (iLayout = 0; iLayout < _cutEngine.LayoutCount; iLayout++)
            {
                _cutEngine.GetLayoutInfo(iLayout, out sheetIndex, out StockCount);

                // sheetIndex is global index of the first sheet used in the layout iLayout
                // StockCount is quantity of sheets of the same size as sheetIndex used for this layout
                if (StockCount > 0)
                {
                    // Output information about each stock, such as stock Length
                    for (iSheet = sheetIndex; iSheet < sheetIndex + StockCount; iSheet++)
                    {
                        if (_cutEngine.GetStockInfo(iSheet, out width, out height, out sheetActive))
                        {
                            field = field + (width * height);

                            // Output the information about parts cut from this sheet
                            // First we get quantity of parts cut from the sheet
                            partCount = _cutEngine.GetPartCountOnStock(iSheet);

                            // Iterate by parts and get indices of cut parts
                            for (iPart = 0; iPart < partCount; iPart++)
                            {
                                // Get global part index of iPart cut from the current sheet
                                partIndex = _cutEngine.GetPartIndexOnStock(iSheet, iPart);

                                // Get sizes and location of the source part with index partIndex
                                if (_cutEngine.GetResultPart(partIndex, out tmp, out W, out H, out X, out Y, out rotated))
                                {
                                    usedField = usedField + (X * Y);                                    
                                }
                            }
                        }
                    }
                }
            }

            wasteField = field - usedField;

        }
    }
}
