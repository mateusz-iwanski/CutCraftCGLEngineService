using CutCraftEngineData.DataOutput;
using CutGLib;
using NLog.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataOutput
{
    /// <summary>
    /// Layouts 2D statistics from CutGLib results.
    /// 
    /// The result is a list of statistics for each layout.
    /// </summary>
    public class CGLLayoutStatistics2DFactory : ICGLDataOutputFactory<CGLStatistics2D>
    {
        private readonly CutEngine _cutEngine;

        private List<CGLStatistics2D> _statistics = new List<CGLStatistics2D>();
        private readonly List<CGLLayout> _layouts;
        private readonly List<CGLSheet> _sheets;

        public CGLLayoutStatistics2DFactory(CutEngine cutEngine, List<CGLLayout> cGLLayouts, List<CGLSheet> cGLSheets)
        {
            _cutEngine = cutEngine;
            _layouts = cGLLayouts;
            _sheets = cGLSheets;
            
            Build();

            return;
        }

        /// <summary>
        /// Add statistics for each layout to the list.
        /// </summary>
        private void Build()
        {
            int sheetIndex, StockCount, iPart, iLayout, partCount, partIndex, tmp, iSheet;
            double width, height, X, Y, W, H;
            bool rotated, sheetActive;

            // Iterate by each layout
            // such as number and length of used stocks and part indices cut from the stocks
            for (iLayout = 0; iLayout < _cutEngine.LayoutCount; iLayout++)
            {
                _cutEngine.GetLayoutInfo(iLayout, out sheetIndex, out StockCount);
                
                if (StockCount > 0)
                {
                    var _layout = _layouts.FirstOrDefault(l => l.Layout == iLayout) ??
                        throw new Exceptions.CustomException($"Can't find Layout index ({iLayout})");

                    var _field = 0d;
                    var _usedField = 0d;

                    // iSheet is stock number, every stock can have multiple (StockCount) sheets (cut in package)
                    // Iterate by each stock
                    for (iSheet = sheetIndex; iSheet < sheetIndex + StockCount; iSheet++)
                    {
                        if (_cutEngine.GetStockInfo(iSheet, out width, out height, out sheetActive))
                        {

                            _field += width * height;

                            var _sheet = _sheets.FirstOrDefault(s => s.Sheet == iSheet) ??
                                throw new Exceptions.CustomException($"Can't find Sheet index ({iSheet})");

                            var _cutCount = _cutEngine.GetStockCutCount(iSheet);
                            var _cutTrimCount = _cutEngine.GetStockTrimCutCount(iSheet);

                            double _cutLength = 0d;
                            double _cutTrimLength = 0d;

                            partCount = _cutEngine.GetPartCountOnStock(iSheet);
                            
                            /// count used field
                            // Iterate by parts and get indices of cut parts
                            for (iPart = 0; iPart < partCount; iPart++)
                            {
                                // Get global part index of iPart cut from the current sheet
                                partIndex = _cutEngine.GetPartIndexOnStock(iSheet, iPart);

                                // Get sizes and location of the source part with index partIndex
                                if (_cutEngine.GetResultPart(partIndex, out tmp, out W, out H, out X, out Y, out rotated))
                                {
                                    _usedField += W * H;
                                }
                            }

                            /// count trim cut length
                            var CutsCount = _cutEngine.GetStockTrimCutCount(iSheet);
                            for (var iCut = 0; iCut < CutsCount; iCut++)
                            {
                                double X1, Y1, X2, Y2;

                                _cutEngine.GetStockTrimCut(iSheet, iCut, out X1, out Y1, out X2, out Y2);

                                _cutTrimLength += CGLCut.calculateDistance(X1, Y1, X2, Y2);
                            }
                            
                            /// count cut length
                            CutsCount = _cutEngine.GetStockCutCount(iSheet);
                            for (var iCut = 0; iCut < CutsCount; iCut++)
                            {
                                double X1, Y1, X2, Y2;

                                _cutEngine.GetStockCut(iSheet, iCut, out X1, out Y1, out X2, out Y2);

                                _cutLength += CGLCut.calculateDistance(X1, Y1, X2, Y2);
                            }

                            _statistics.Add(new CGLStatistics2D
                                {
                                    field = _field,
                                    usedField = _usedField,
                                    wasteField = _field - _usedField,  // CutGLib doesn't have waste field
                                    unusedField = _field - _usedField,
                                    cutCount = _cutCount,
                                    cutsLength = _cutLength,
                                    CutTrimCount = _cutTrimCount,
                                    CutsTrimLength = _cutTrimLength,
                                    Layout = _layout,
                                    Sheet = _sheet
                                });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// return the list of statistics for layouts
        /// </summary>
        public List<CGLStatistics2D> Get() => _statistics;

    }
}
