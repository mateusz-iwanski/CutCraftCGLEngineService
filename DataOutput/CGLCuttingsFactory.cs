using CutCraftEngineData.DataOutput;
using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataOutput
{
    /// <summary>
    /// Build list of DataOutput Cutting from CutGlib.CutEngine data output results
    /// 
    /// Cutting object has properties :
    ///     StockItemId
    ///     quantity 
    ///     statistics
    ///     pieces
    ///     rest
    ///     cuts
    /// </summary>
    /// <remarks>To prepare properties, use factories</remarks>

    public class CGLCuttingsFactory : ICGLDataOutputFactory<Cutting>
    {
        private readonly CutEngine _cutEngine;

        private readonly CGLSheetFactory _sheetFactory;
        private CGLCutFactory _cutFactory { get; set; }
        private readonly CGLPartFactory _partFactory;
        private readonly CGLLayoutFactory _layoutFactory;
        private readonly CGLLayoutStatistics2DFactory _statistics2DFactory;

        private List<Cutting> _cuttings { get; set; }


        public CGLCuttingsFactory(CutEngine cutEngine)
        {
            _cutEngine = cutEngine;

            _sheetFactory = new CGLSheetFactory(_cutEngine);
            _partFactory = new CGLPartFactory(_cutEngine, _sheetFactory);
            _layoutFactory = new CGLLayoutFactory(_cutEngine, _sheetFactory, _partFactory);
            _cutFactory = new CGLCutFactory(_cutEngine, _sheetFactory, _layoutFactory);
            _statistics2DFactory = new CGLLayoutStatistics2DFactory(_cutEngine, _layoutFactory, _sheetFactory);

            Build();

            return;
        }

        private List<Cutting> Build()
        {
            _cuttings = new List<Cutting>();

            foreach (var layout in _layoutFactory.Get())
            {
                // get statistcs for layout
                var statistics2DFactory = _statistics2DFactory.Get().First(s => s.Layout.Layout == layout.Layout);  // FirstOrDefault(s => s.Layout.Layout == layout.Layout);
                CuttingStatistics cuttingStatisticsMapper = new CuttingStatistics() { _2d = new CGLCutting2DStatisticsMapper(statistics2DFactory).Map() };

                // get parts for layout
                List<CuttingPiece> piecesInLayoutMapper = new CGLPiecesMapper(layout.Parts).Map();

                // get cuts for layout
                var cutsInLayout = _cutFactory.Get().Where(c => c.Layout.Layout == layout.Layout).ToList();
                List<Cut> cuts = new CGLCutsMapper(cutsInLayout).Map();

                _cuttings.Add(new Cutting()
                {
                    stockItemId = layout.StartSheet.StockItemId,
                    quantity = layout.CountOfSheets,
                    statistics = cuttingStatisticsMapper,
                    pieces = piecesInLayoutMapper,
                    rest = new List<CuttingRest>(),
                    cuts = cuts
                });
            }

            return _cuttings;
        }

        public List<Cutting> Get() => _cuttings;

    }
}
