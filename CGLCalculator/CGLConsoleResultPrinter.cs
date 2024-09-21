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
        private readonly CutEngine CutEngine;

        public CGLConsoleResultPrinter(ICGLCalculator cutEngine)
        {
            this.CutEngine = cutEngine.GetCutEngine();

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

            Console.WriteLine("Used {0} sheets", CutEngine.UsedStockCount);
            
            // Output guilltoine cuts for each sheet
            for (StockNo = 0; StockNo < CutEngine.StockCount; StockNo++)
            {
                CutEngine.GetStockInfo(StockNo, out Width, out Height, out active);
                // Sheet was not used during calculation
                if (!active)
                {
                    Console.WriteLine("Sheet={0} was not used.", StockNo);
                    continue;
                }
                Console.WriteLine("Sheet={0}: Width={1} Height={2}", StockNo, Width, Height);
                // First output any trim cuts for the sheet StockNo
                CutsCount = CutEngine.GetStockTrimCutCount(StockNo);
                for (iCut = 0; iCut < CutsCount; iCut++)
                {
                    CutEngine.GetStockTrimCut(StockNo, iCut, out X1, out Y1, out X2, out Y2);
                    Console.WriteLine("Trim  Cut={0}:  X1={1};  Y1={2};  X2={3};  Y2={4}", iCut, X1, Y1, X2, Y2);
                }
                // Now output any actual cuts for the sheet StockNo
                CutsCount = CutEngine.GetStockCutCount(StockNo);
                for (iCut = 0; iCut < CutsCount; iCut++)
                {
                    CutEngine.GetStockCut(StockNo, iCut, out X1, out Y1, out X2, out Y2);
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

            Console.WriteLine("Part Count={0}", CutEngine.PartCount);
            for (iPart = 0; iPart < CutEngine.PartCount; iPart++)
            {
                // Get sizes and location of the source part with index Iter
                // in case of incomplete optimization the part can be unplaced
                // and the function returns FALSE.
                if (CutEngine.GetResultPart(iPart, out StockNo, out W, out H, out X, out Y, out Rotated, out id))
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

            Console.WriteLine("Used {0} sheets", CutEngine.UsedStockCount);
            Console.WriteLine("Created {0} different layouts", CutEngine.LayoutCount);
            // Iterate by each layout and output information about each layout,
            // such as number and length of used stocks and part indices cut from the stocks
            for (iLayout = 0; iLayout < CutEngine.LayoutCount; iLayout++)
            {
                CutEngine.GetLayoutInfo(iLayout, out sheetIndex, out StockCount);
                // sheetIndex is global index of the first sheet used in the layout iLayout
                // StockCount is quantity of sheets of the same size as sheetIndex used for this layout
                if (StockCount > 0)
                {
                    // Uncomment and change the file name if required
                    // CutEngine.CreateStockImage(sheetIndex, string.Format("d:\\Panel_Layout_{0}.jpg", iLayout + 1), 2000);

                    Console.WriteLine("Layout={0}:  Start Sheet={1};  Count of Sheet={2}", iLayout, sheetIndex, StockCount);
                    // Output information about each stock, such as stock Length
                    for (iSheet = sheetIndex; iSheet < sheetIndex + StockCount; iSheet++)
                    {
                        if (CutEngine.GetStockInfo(iSheet, out width, out height, out sheetActive))
                        {
                            Console.WriteLine("Sheet={0}:  Width={1}; Height={2}", iSheet, width, height);
                            // Output the information about parts cut from this sheet
                            // First we get quantity of parts cut from the sheet
                            partCount = CutEngine.GetPartCountOnStock(iSheet);
                            // Iterate by parts and get indices of cut parts
                            for (iPart = 0; iPart < partCount; iPart++)
                            {
                                // Get global part index of iPart cut from the current sheet
                                partIndex = CutEngine.GetPartIndexOnStock(iSheet, iPart);
                                // Get sizes and location of the source part with index partIndex
                                if (CutEngine.GetResultPart(partIndex, out tmp, out W, out H, out X, out Y, out rotated))
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

            Console.WriteLine("Used {0} sheets", CutEngine.UsedStockCount);
            Console.WriteLine("Created {0} different layouts", CutEngine.LayoutCount);
            for (int iLayout = 0; iLayout < CutEngine.LayoutCount; iLayout++)
            {
                CutEngine.GetLayoutInfo(iLayout, out StockNo, out StockCount);
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
            Console.WriteLine("OUTPUT CALCULATOR SETTINGS");
            Console.WriteLine("======================================");

            Console.WriteLine("TrimLeft - {0}", CutEngine.TrimLeft);
            Console.WriteLine("TrimTop - {0}", CutEngine.TrimTop);
            Console.WriteLine("TrimRight - {0}", CutEngine.TrimRight);
            Console.WriteLine("TrimBottom - {0}", CutEngine.TrimBottom);
            Console.WriteLine("SawWidth - {0}", CutEngine.SawWidth);
            Console.WriteLine("MinimizeSheetRotation - {0}", CutEngine.MinimizeSheetRotation);
            Console.WriteLine("MaxCutLevel - {0}", CutEngine.MaxCutLevel);
            Console.WriteLine("CompleteMode - {0}", CutEngine.CompleteMode);
            Console.WriteLine("UseLayoutMinimization - {0}", CutEngine.UseLayoutMinimization);
            Console.WriteLine("MaxLayoutSize - {0}", CutEngine.MaxLayoutSize);
            Console.WriteLine("WasteSizeMin - {0}", CutEngine.WasteSizeMin);
            Console.WriteLine("Maximal size of the stock={0}", CutEngine.MaxSizeSizes);
            Console.WriteLine("Maximal size of the part={0}", CutEngine.MaxPartSizes);
            Console.WriteLine("Maximal number of decimal points={0}", CutEngine.MaxDecimalPoint);
            Console.WriteLine("Allow combine stock regular and waste={0}", CutEngine.AllowCombineStockRegularAndWaste);
            Console.WriteLine("Use large stock first={0}", CutEngine.UseLargeStockFirst);            
            Console.WriteLine("Is linear run={0}", CutEngine.IsLinearRun);
            Console.WriteLine("Remaining part count={0}", CutEngine.RemainingPartCount);
            Console.WriteLine("Used linear stock count={0}", CutEngine.UsedLinearStockCount);            
            Console.WriteLine("Stock count={0}", CutEngine.StockCount);
            Console.WriteLine("Part count={0}", CutEngine.PartCount);
            Console.WriteLine("Elapsed time={0}", CutEngine.ElapsedTime);
            Console.WriteLine("Placed part count={0}", CutEngine.PlacedPartCount);            
            Console.WriteLine("Version={0}", CutEngine.Version);
            Console.WriteLine();
        }
    }
}
