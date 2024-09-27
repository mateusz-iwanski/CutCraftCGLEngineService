using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataOutput
{
    /// <summary>
    /// Sheet from CutGLib results
    /// </summary>
    public class CGLSheet
    {
        /// <summary>
        /// Sheet number
        /// </summary>
        public int Sheet { get; init; }

        /// <summary>
        /// Width 
        /// </summary>
        public double Width { get; init; }

        /// <summary>
        /// Height
        /// </summary>
        public double Height { get; init; }

        public int StockItemId { get; init; }
    }
}
