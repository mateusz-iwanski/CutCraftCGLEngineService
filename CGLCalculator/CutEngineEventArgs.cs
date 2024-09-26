using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.CGLCalculator
{
    /// <summary>
    /// Custom event arguments for CutEngine event.
    /// </summary>
    public class CutEngineEventArgs : EventArgs
    {
        public CutEngine CutEngine { get; }

        public CutEngineEventArgs(CutEngine cutEngine)
        {
            CutEngine = cutEngine;
        }
    }
}
