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
    /// Mapping CutGlib data output to list of DataOutput Cutting
    /// </summary>

    public class CuttingsFactory : ICGLDataOutputFactory<Cutting>
    {
        private readonly CutEngine _cutEngine;

        private readonly CGLSheetFactory _sheetFactory;
        private CGLCutFactory _cutFactory { get; set; }
        private readonly CGLPartFactory _partFactory;
        private readonly CGLLayoutFactory _layoutFactory;
        private readonly CGLLayoutStatistics2DFactory _statistics2DFactory;

        private List<Cutting> _cuttings { get; set; }


        public CuttingsFactory(CutEngine cutEngine)
        {
            _cutEngine = cutEngine;

            _sheetFactory = new CGLSheetFactory(_cutEngine);
            _partFactory = new CGLPartFactory(_cutEngine, _sheetFactory);
            _layoutFactory = new CGLLayoutFactory(_cutEngine, _sheetFactory, _partFactory);
            _cutFactory = new CGLCutFactory(_cutEngine, _sheetFactory, _layoutFactory);
            _statistics2DFactory = new CGLLayoutStatistics2DFactory(_cutEngine, _layoutFactory, _sheetFactory);

            Build();
        }

        private List<Cutting> Build()
        {
            _cuttings = new List<Cutting>();

            foreach (var layout in _layoutFactory.Get())
            {
                var a = _statistics2DFactory.Get();

                // get statistcs for layout
                var statistics2DFactory = _statistics2DFactory.Get().First(s => s.Layout.Layout == layout.Layout);  // FirstOrDefault(s => s.Layout.Layout == layout.Layout);
                CuttingStatistics cuttingStatisticsMapper = new CuttingStatistics() { _2d = new Cutting2DStatisticsMapper(statistics2DFactory).Map() };

                // get parts for layout
                List<CuttingPiece> piecesInLayoutMapper = new PiecesMapper(layout.Parts).Map();

                // get cuts for layout
                var cutsInLayout = _cutFactory.Get().Where(c => c.Layout.Layout == layout.Layout).ToList();
                List<Cut> cuts = new CutsMapper(cutsInLayout).Map();

                _cuttings.Add(new Cutting()
                {
                    stockItemId = layout.Layout,
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
