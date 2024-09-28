using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.DataOutput
{
    /// <summary>
    /// Build layout from CutGLib results
    /// </summary>
    public class CGLLayoutFactory : ICGLDataOutputFactory<CGLLayout>
    {
        private readonly CutEngine _cutEngine;

        private List<CGLLayout> _layout;
        private List<CGLSheet> _sheets;
        private List<CGLPart> _parts;

        public CGLLayoutFactory(CutEngine cutEngine, ICGLDataOutputFactory<CGLSheet> sheets, ICGLDataOutputFactory<CGLPart> parts)
        {
            _cutEngine = cutEngine;

            _layout = new List<CGLLayout>();
            _sheets = sheets.Get();
            _parts = parts.Get();

            Build();
        }

        /// <summary>
        /// Add layouts to the list of CGLLayout
        /// </summary>
        private void Build()
        {
            int sheetIndex, StockCount, iLayout;            

            // Iterate by each layout and output information about each layout,
            // such as number and length of used stocks and part indices cut from the stocks
            for (iLayout = 0; iLayout < _cutEngine.LayoutCount; iLayout++)
            {
                _cutEngine.GetLayoutInfo(iLayout, out sheetIndex, out StockCount);
                // sheetIndex is global index of the first sheet used in the layout iLayout
                // StockCount is quantity of sheets of the same size as sheetIndex used for this layout
                if (StockCount > 0)
                {
                    var startSheet = _sheets.FirstOrDefault(s => s.Sheet == sheetIndex) ?? 
                        throw new Exceptions.CustomException($"Can't find Sheet index ({sheetIndex})");

                    var parts = _parts.Where(p => p.Sheet.Sheet == sheetIndex).ToList();

                    _layout.Add(new CGLLayout
                    {
                        Layout = iLayout,
                        StartSheet = startSheet,
                        CountOfSheets = StockCount,
                        Parts = parts
                    });
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Get layouts list
        /// </summary>
        public List<CGLLayout> Get() => _layout;
    }
}
