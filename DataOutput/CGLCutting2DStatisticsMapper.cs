using CutCraftEngineData.DataOutput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataOutput
{
    public class CGLCutting2DStatisticsMapper
    {
        CGLStatistics2D _statics2D;

        public CGLCutting2DStatisticsMapper(CGLStatistics2D statics2D)
        {
            _statics2D = statics2D;
            return;
        }

        public CuttingStatistics2d Map()
        {
            return new CuttingStatistics2d()
            {
                field = _statics2D.field,
                usedField = _statics2D.usedField,
                wasteField = _statics2D.wasteField,
                unusedField = _statics2D.unusedField,
                cutCount = _statics2D.cutCount,
                cutsLength = _statics2D.cutsLength
            };
        }
    }
}
