using CutCraftEngineData.DataInput;
using CutCraftEngineData.DataOutput;
using CutCraftEngineWebSocketCGLService.CGLCalculator;
using CutGLib;
using NLog.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CutCraftEngineWebSocketCGLService.DataOutput
{
    /// <summary>
    /// 2D statistics from CutGLib results.
    /// </summary>
    public class CGLStatistics2D : Statistics2d
    {
        /// <summary>
        /// The number of trim cuts.
        /// </summary>
        public int CutTrimCount { get; init; }

        /// <summary>
        /// The total length of all trim cuts
        /// </summary>
        public double CutsTrimLength { get; init; }

        /// <summary>
        /// The layout in which the statistics are calculated
        /// </summary>
        public CGLLayout Layout { get; init; }

        /// <summary>
        /// Sheet in layout
        /// 
        /// Layout has only one sheet with specific width and height
        /// </summary>
        public CGLSheet Sheet { get; init; }
    }
}
