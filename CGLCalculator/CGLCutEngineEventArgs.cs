using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.CGLCalculator
{
    /// <summary>
    /// Custom event arguments for CutEngine event.
    /// </summary>
    public class CGLCutEngineEventArgs : EventArgs
    {
        public CutEngine CutEngine { get; }

        public CGLCutEngineEventArgs(CutEngine cutEngine)
        {
            CutEngine = cutEngine;
        }
    }
}
