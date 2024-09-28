using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.DataOutput
{
    /// <summary>
    /// Build parts from CutGLib results
    /// </summary>
    public class CGLPartFactory : ICGLDataOutputFactory<CGLPart>
    {
        private readonly CutEngine _cutEngine;

        private List<CGLPart> _parts = new List<CGLPart>();
        private readonly List<CGLSheet> _sheets;

        public CGLPartFactory(CutEngine cutEngine, ICGLDataOutputFactory<CGLSheet> sheets)
        {
            _cutEngine = cutEngine;
            _sheets = sheets.Get();

            Build();

            return;
        }

        /// <summary>
        /// Add parts to the list of CGLPart
        /// </summary>
        private void Build()
        {
            int sheetIndex, StockCount, iPart, iLayout, partCount, partIndex, tmp, iSheet;
            double width, height, X, Y, W, H;
            bool rotated, sheetActive;
            string id;

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
                        if (_cutEngine.GetStockInfo(iSheet, out width, out height, out sheetActive))
                        {
                            // Output the information about parts cut from this sheet
                            // First we get quantity of parts cut from the sheet
                            partCount = _cutEngine.GetPartCountOnStock(iSheet);
                            
                            // Iterate by parts and get indices of cut parts
                            for (iPart = 0; iPart < partCount; iPart++)
                            {
                                // Get global part index of iPart cut from the current sheet
                                partIndex = _cutEngine.GetPartIndexOnStock(iSheet, iPart);
                                
                                // Get sizes and location of the source part with index partIndex
                                if (_cutEngine.GetResultPart(partIndex, out tmp, out W, out H, out X, out Y, out rotated, out id))
                                {
                                    int _id;
                                    
                                    if (!Int32.TryParse(id, out _id))
                                        throw new Exceptions.ArgumentException($"Can't Part id {id} convert to int");

                                    _parts.Add(new CGLPart
                                    {
                                        Id = _id,
                                        PartId = partIndex,
                                        Sheet = _sheets.FirstOrDefault(s => s.Sheet == iSheet) ?? throw new Exceptions.CustomException($"Can't find Sheet index ({iSheet})"),
                                        X = X,
                                        Y = Y,
                                        Width = W,
                                        Height = H,
                                        Rotated = rotated,
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get parts list
        /// </summary>
        public List<CGLPart> Get() => _parts;
    }
}
