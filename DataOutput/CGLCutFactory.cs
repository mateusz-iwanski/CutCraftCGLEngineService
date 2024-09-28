using CutGLib;
using NLog.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.DataOutput
{
    /// <summary>
    /// Build CGLCut from CutEngine
    /// </summary>
    public class CGLCutFactory : ICGLDataOutputFactory<CGLCut>
    {
        private readonly CutEngine _cutEngine;

        private List<CGLCut> _cuts = new List<CGLCut>();
        private readonly List<CGLSheet> _sheets;
        private readonly List<CGLLayout> _layouts;

        public CGLCutFactory(CutEngine cutEngine, ICGLDataOutputFactory<CGLSheet> sheets, ICGLDataOutputFactory<CGLLayout> layout)
        {
            _cutEngine = cutEngine;
            _sheets = sheets.Get();
            _layouts = layout.Get();

            Build();

            return;
        }

        /// <summary>
        /// Add cuts to the list of CGLCut
        /// </summary>
        private void Build()
        {
            int StockNo, iCut;
            long CutsCount;
            double Width, Height, X1 = 0, Y1 = 0, X2 = 0, Y2 = 0;
            bool active;
            string aID;

            for (StockNo = 0; StockNo < _cutEngine.StockCount; StockNo++)
            {
                _cutEngine.GetStockInfo(StockNo, out Width, out Height, out active, out aID);

                // only the sheet used in the layout
                if (active)
                {
                    var sheet = _sheets.FirstOrDefault(s => s.Sheet == StockNo) ?? throw new Exceptions.CustomException($"Can't find Sheet index ({StockNo})");
                    var layout = _layouts.FirstOrDefault(_layouts => _layouts.Parts.Any(p => p.Sheet == sheet)) ?? throw new Exceptions.CustomException($"Can't find in list of layouts Sheet index {sheet.Sheet}");

                    // First output any trim cuts for the sheet StockNo
                    CutsCount = _cutEngine.GetStockTrimCutCount(StockNo);
                    for (iCut = 0; iCut < CutsCount; iCut++)
                    {
                        _cutEngine.GetStockTrimCut(StockNo, iCut, out X1, out Y1, out X2, out Y2);
                        _cuts.Add(new CGLCut
                        {
                            Cut = iCut,
                            X1 = X1,
                            Y1 = Y1,
                            X2 = X2,
                            Y2 = Y2,
                            Sheet = sheet,
                            Layout = layout,
                            Trim = true
                        });
                    }

                    // Now output any actual cuts for the sheet StockNo
                    CutsCount = _cutEngine.GetStockCutCount(StockNo);
                    for (iCut = 0; iCut < CutsCount; iCut++)
                    {
                        _cutEngine.GetStockCut(StockNo, iCut, out X1, out Y1, out X2, out Y2);

                        _cuts.Add(new CGLCut
                        {
                            Cut = iCut,
                            X1 = X1,
                            Y1 = Y1,
                            X2 = X2,
                            Y2 = Y2,
                            Sheet = sheet,
                            Layout = layout,
                            Trim = false
                        });

                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Get cuts list
        /// </summary>
        public List<CGLCut> Get() => _cuts;
    }
}
