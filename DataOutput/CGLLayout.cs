using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataOutput
{
    /// <summary>
    /// Layout from CutGLib results
    /// </summary>
    public class CGLLayout
    {
        /// <summary>
        /// The layout number
        /// </summary>
        public int Layout { get; init; }

        /// <summary>
        /// The sheet on which the first parts is placed
        /// </summary>
        public CGLSheet StartSheet { get; init; }

        /// <summary>
        /// List of sheets in the layout
        /// </summary>
        public List<CGLSheet> Sheets { get; init; }

        /// <summary>
        /// The number of sheets placed in the stack when cutting (cut in package)
        /// </summary>
        public int CountOfSheets { get; init; }

        /// <summary>
        /// List of parts placed on the sheet/s
        /// </summary>
        public List<CGLPart> Parts { get; init; } 

    }
}
