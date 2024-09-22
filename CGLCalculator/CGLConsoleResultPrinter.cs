using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.CGLCalculator
{
    /// <summary>
    /// Print cutting optimization results with calculator settings on the console
    /// </summary>
    public class CGLConsoleResultPrinter
    {
        private readonly CutEngine _cutEngine;

        public CGLConsoleResultPrinter(CGLCalculator calculator)
        {
            if (calculator.IsExecuted() == false)
                throw new InvalidOperationException("The calculator must be executed before printing the result to the console.");

            _cutEngine = calculator.GetCutEngine();

            PrinSheetByParts();
            PrintSheetByLayout();
            PrintLayoutInfo();
            PrintCalculatorSettings();
        }

        private void PrinSheetByParts()
        {
            int StockNo, iCut, iPart;
            long CutsCount;
            double Width, Height, X1 = 0, Y1 = 0, X2 = 0, Y2 = 0;
            bool active;
            string id;
            Console.WriteLine("======================================");
            Console.WriteLine("OUTPUT CUTS RESULTS");
            Console.WriteLine("======================================");

            Console.WriteLine("Used {0} sheets", _cutEngine.UsedStockCount);
            
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

        // This rotine outputs the results for 2D cutting optimization by layouts
        private void PrintSheetByLayout()
        {
            int sheetIndex, StockCount, iPart, iLayout, partCount, partIndex, tmp, iSheet;
            double width, height, X, Y, W, H;
            bool rotated, sheetActive;
            string Txt;

            Console.WriteLine("======================================");
            Console.WriteLine("OUTPUT LAYOUT RESULTS");
            Console.WriteLine("======================================");

            Console.WriteLine("Used {0} sheets", _cutEngine.UsedStockCount);
            Console.WriteLine("Created {0} different layouts", _cutEngine.LayoutCount);
            // Iterate by each layout and output information about each layout,
            // such as number and length of used stocks and part indices cut from the stocks
            for (iLayout = 0; iLayout < _cutEngine.LayoutCount; iLayout++)
            {
                _cutEngine.GetLayoutInfo(iLayout, out sheetIndex, out StockCount);
                // sheetIndex is global index of the first sheet used in the layout iLayout
                // StockCount is quantity of sheets of the same size as sheetIndex used for this layout
                if (StockCount > 0)
                {
                    // Uncomment and change the file name if required
                    // _cutEngine.CreateStockImage(sheetIndex, string.Format("d:\\Panel_Layout_{0}.jpg", iLayout + 1), 2000);

                    Console.WriteLine("Layout={0}:  Start Sheet={1};  Count of Sheet={2}", iLayout, sheetIndex, StockCount);
                    // Output information about each stock, such as stock Length
                    for (iSheet = sheetIndex; iSheet < sheetIndex + StockCount; iSheet++)
                    {
                        if (_cutEngine.GetStockInfo(iSheet, out width, out height, out sheetActive))
                        {
                            Console.WriteLine("Sheet={0}:  Width={1}; Height={2}", iSheet, width, height);
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
                                    // Output the coordinates
                                    if (rotated) Txt = "Yes";
                                    else Txt = "No";
                                    Console.WriteLine("Part={0}; sheet={1}; Width={2}; Height={3}; X={4}; Y={5}; Rotated={6}",
                                                  partIndex, iSheet, W, H, X, Y, Txt);

                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine();
        }

        // Outputs the layout information.
        private void PrintLayoutInfo()
        {
            int StockNo, StockCount;

            Console.WriteLine("======================================");
            Console.WriteLine("OUTPUT LAYOUT INFO");
            Console.WriteLine("======================================");

            Console.WriteLine("Used {0} sheets", _cutEngine.UsedStockCount);
            Console.WriteLine("Created {0} different layouts", _cutEngine.LayoutCount);
            for (int iLayout = 0; iLayout < _cutEngine.LayoutCount; iLayout++)
            {
                _cutEngine.GetLayoutInfo(iLayout, out StockNo, out StockCount);
                if (StockCount > 0)
                {
                    Console.WriteLine("Layout={0}:  Start Sheet={1};  Count of Sheets={2}", iLayout, StockNo, StockCount);
                }
            }
            Console.WriteLine();
        }

        // Outputs the calculator settings.
        private void PrintCalculatorSettings()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("CALCULATOR SETTINGS");
            Console.WriteLine("======================================");

            Console.WriteLine("TrimLeft - {0}", _cutEngine.TrimLeft);
            Console.WriteLine("TrimTop - {0}", _cutEngine.TrimTop);
            Console.WriteLine("TrimRight - {0}", _cutEngine.TrimRight);
            Console.WriteLine("TrimBottom - {0}", _cutEngine.TrimBottom);
            Console.WriteLine("SawWidth - {0}", _cutEngine.SawWidth);
            Console.WriteLine("MinimizeSheetRotation - {0}", _cutEngine.MinimizeSheetRotation);
            Console.WriteLine("MaxCutLevel - {0}", _cutEngine.MaxCutLevel);
            Console.WriteLine("CompleteMode - {0}", _cutEngine.CompleteMode);
            Console.WriteLine("UseLayoutMinimization - {0}", _cutEngine.UseLayoutMinimization);
            Console.WriteLine("MaxLayoutSize - {0}", _cutEngine.MaxLayoutSize);
            Console.WriteLine("WasteSizeMin - {0}", _cutEngine.WasteSizeMin);
            Console.WriteLine("Maximal size of the stock={0}", _cutEngine.MaxSizeSizes);
            Console.WriteLine("Maximal size of the part={0}", _cutEngine.MaxPartSizes);
            Console.WriteLine("Maximal number of decimal points={0}", _cutEngine.MaxDecimalPoint);
            Console.WriteLine("Allow combine stock regular and waste={0}", _cutEngine.AllowCombineStockRegularAndWaste);
            Console.WriteLine("Use large stock first={0}", _cutEngine.UseLargeStockFirst);            
            Console.WriteLine("Is linear run={0}", _cutEngine.IsLinearRun);
            Console.WriteLine("Remaining part count={0}", _cutEngine.RemainingPartCount);
            Console.WriteLine("Used linear stock count={0}", _cutEngine.UsedLinearStockCount);            
            Console.WriteLine("Stock count={0}", _cutEngine.StockCount);
            Console.WriteLine("Part count={0}", _cutEngine.PartCount);
            Console.WriteLine("Elapsed time={0}", _cutEngine.ElapsedTime);
            Console.WriteLine("Placed part count={0}", _cutEngine.PlacedPartCount);            
            Console.WriteLine("Version={0}", _cutEngine.Version);
            Console.WriteLine();
        }
    }
}
