using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.DataOutput
{
    /// <summary>
    /// Build CGLSheet objects from CutEngine
    /// </summary>
    internal class CGLSheetFactory : ICGLDataOutputFactory<CGLSheet>
    {
        private readonly CutEngine _cutEngine;
        private List<CGLSheet> _sheets = new List<CGLSheet>();

        public CGLSheetFactory(CutEngine cutEngine)
        {
            _cutEngine = cutEngine;

            Build();

            return;
        }

        /// <summary>
        /// Add sheet to the list of CGLSheet
        /// </summary>
        private void Build()
        {
            int sheetIndex, StockCount, iLayout, iSheet;
            double width, height;
            string aID;
            bool sheetActive;

            // Iterate by each layout and output information about each layout,
            // such as number and length of used stocks and part indices cut from the stocks
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
                        if (_cutEngine.GetStockInfo(iSheet, out width, out height, out sheetActive, out aID))
                        {
                            _sheets.Add(new CGLSheet
                            {
                                Sheet = iSheet,
                                Width = width,
                                Height = height,
                                StockItemId = int.TryParse(aID, out int stockItemId) ? stockItemId : throw new Exceptions.CustomException($"Can't parse StockItemId ({aID})")
                            });
                        }
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Return list of CGLSheet
        /// </summary>
        public List<CGLSheet> Get()
        {
            return _sheets;
        }
    }
}
