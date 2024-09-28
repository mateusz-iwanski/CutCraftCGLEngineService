using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.CGLCalculator
{
    /// <summary>
    /// Printer cutting optimization results with calculator settings with a specific format.
    /// </summary>
    /// <remarks>
    /// Use by CGLDisplayLogger.
    /// It is used to print the result to the console whenever Execute is called by CutEngine.
    /// </remarks>
    public class CGLResultPrinter
    {
        private readonly CutEngine _cutEngine;
        private readonly Action<string> _display;

        public CGLResultPrinter(CGLCalculator calculator, Action<string> displayDelegate)
        {
            if (calculator.IsExecuted() == false)
                throw new InvalidOperationException("The calculator must be executed before printing the result to the console.");

            _cutEngine = calculator.GetCutEngine();
            _display = displayDelegate ?? throw new ArgumentNullException(nameof(displayDelegate));

            _display("Start ==============");
            PrinSheetByParts();
            PrintSheetByLayout();
            PrintLayoutInfo();
            PrintCalculatorSettings();
            _display("============== End");
        }

        private void PrinSheetByParts()
        {
            int StockNo, iCut, iPart;
            long CutsCount;
            double Width, Height, X1 = 0, Y1 = 0, X2 = 0, Y2 = 0;
            bool active;
            string id;
            
            _display("======================================");
            _display("OUTPUT CUTS RESULTS");
            _display("======================================");

            _display($"Used {_cutEngine.UsedStockCount} sheets");

            // Output guilltoine cuts for each sheet
            for (StockNo = 0; StockNo < _cutEngine.StockCount; StockNo++)
            {
                _cutEngine.GetStockInfo(StockNo, out Width, out Height, out active);
                // Sheet was not used during calculation
                if (!active)
                {
                    _display($"Sheet={StockNo} was not used.");
                    continue;
                }
                _display($"Sheet={StockNo}: Width={Width} Height={Height}");
                // First output any trim cuts for the sheet StockNo
                CutsCount = _cutEngine.GetStockTrimCutCount(StockNo);
                for (iCut = 0; iCut < CutsCount; iCut++)
                {
                    _cutEngine.GetStockTrimCut(StockNo, iCut, out X1, out Y1, out X2, out Y2);
                    _display($"Trim  Cut={iCut}:  X1={X1};  Y1={Y1};  X2={X2};  Y2={Y2}");
                }
                // Now output any actual cuts for the sheet StockNo
                CutsCount = _cutEngine.GetStockCutCount(StockNo);
                for (iCut = 0; iCut < CutsCount; iCut++)
                {
                    _cutEngine.GetStockCut(StockNo, iCut, out X1, out Y1, out X2, out Y2);
                    _display($"Cut={iCut}:  X1={X1};  Y1={Y1};  X2={X2};  Y2={Y2}");
                }
            }

            // Get parts locations
            double W = 0, H = 0, X = 0, Y = 0;
            bool Rotated;
            _display("======================================");
            _display("OUTPUT PARTS RESULTS");
            _display("======================================");

            _display($"PartId Count={_cutEngine.PartCount}");
            for (iPart = 0; iPart < _cutEngine.PartCount; iPart++)
            {
                // Get sizes and location of the source part with index Iter
                // in case of incomplete optimization the part can be unplaced
                // and the function returns FALSE.
                if (_cutEngine.GetResultPart(iPart, out StockNo, out W, out H, out X, out Y, out Rotated, out id))
                {
                    _display($"PartId Id={id};  sheet={StockNo};  X={X};  Y={Y};  Width={W};  Height={H}");
                }
                else _display($"PartId {iPart} was not placed");
            }
        }

        // This rotine outputs the results for 2D cutting optimization by layouts
        private void PrintSheetByLayout()
        {
            int sheetIndex, StockCount, iPart, iLayout, partCount, partIndex, tmp, iSheet;
            double width, height, X, Y, W, H;
            bool rotated, sheetActive;
            string Txt;

            _display("======================================");
            _display("OUTPUT LAYOUT RESULTS");
            _display("======================================");

            _display($"Used {_cutEngine.UsedStockCount} sheets");
            _display($"Created {_cutEngine.LayoutCount} different layouts");
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

                    _display($"Layout={iLayout}:  Start Sheet={sheetIndex};  Count of Sheet={StockCount}");
                    // Output information about each stock, such as stock Length
                    for (iSheet = sheetIndex; iSheet < sheetIndex + StockCount; iSheet++)
                    {
                        if (_cutEngine.GetStockInfo(iSheet, out width, out height, out sheetActive))
                        {
                            _display($"Sheet={iSheet}:  Width={width}; Height={height}");
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
                                    _display($"PartId={partIndex}; sheet={iSheet}; Width={W}; Height={H}; X={X}; Y={Y}; Rotated={Txt}");

                                }
                            }
                        }
                    }
                }
            }
        }

        // Outputs the layout information.
        private void PrintLayoutInfo()
        {
            int StockNo, StockCount;

            _display("======================================");
            _display("OUTPUT LAYOUT INFO");
            _display("======================================");

            _display($"Used {_cutEngine.UsedStockCount} sheets");
            _display($"Created {_cutEngine.LayoutCount} different layouts");
            for (int iLayout = 0; iLayout < _cutEngine.LayoutCount; iLayout++)
            {
                _cutEngine.GetLayoutInfo(iLayout, out StockNo, out StockCount);
                if (StockCount > 0)
                {
                    _display($"Layout={iLayout}:  Start Sheet={StockNo};  Count of Sheets={StockCount}");
                }
            }
        }

        // Outputs the calculator settings.
        private void PrintCalculatorSettings()
        {
            _display("======================================");
            _display("CALCULATOR SETTINGS");
            _display("======================================");

            _display($"TrimLeft - {_cutEngine.TrimLeft}");
            _display($"TrimTop - {_cutEngine.TrimTop}");
            _display($"TrimRight - {_cutEngine.TrimRight}");
            _display($"TrimBottom - {_cutEngine.TrimBottom}");
            _display($"SawWidth - {_cutEngine.SawWidth}");
            _display($"MinimizeSheetRotation - {_cutEngine.MinimizeSheetRotation}");
            _display($"MaxCutLevel - {_cutEngine.MaxCutLevel}");
            _display($"CompleteMode - {_cutEngine.CompleteMode}");
            _display($"UseLayoutMinimization - {_cutEngine.UseLayoutMinimization}");
            _display($"MaxLayoutSize - {_cutEngine.MaxLayoutSize}");
            _display($"WasteSizeMin - {_cutEngine.WasteSizeMin}");
            _display($"Maximal size of the stock={_cutEngine.MaxSizeSizes}");
            _display($"Maximal size of the part={_cutEngine.MaxPartSizes}");
            _display($"Maximal number of decimal points={_cutEngine.MaxDecimalPoint}");
            _display($"Allow combine stock regular and waste={_cutEngine.AllowCombineStockRegularAndWaste}");
            _display($"Use large stock first={_cutEngine.UseLargeStockFirst}");
            _display($"Is linear run={_cutEngine.IsLinearRun}");
            _display($"Remaining part count={_cutEngine.RemainingPartCount}");
            _display($"Used linear stock count={_cutEngine.UsedLinearStockCount}");
            _display($"Stock count={_cutEngine.StockCount}");
            _display($"PartId count={_cutEngine.PartCount}");
            _display($"Elapsed time={_cutEngine.ElapsedTime}");
            _display($"Placed part count={_cutEngine.PlacedPartCount}");
            _display($"Version={_cutEngine.Version}");
        }
    }
}
