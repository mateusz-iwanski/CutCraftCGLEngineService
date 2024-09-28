using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.DataInput
{
    /// <summary>
    /// If priority is normal then such stock will be used first before any actual stocks that have low priority. 
    /// 
    /// It is used in the CutEngine.AddStock method.
    /// </summary>
    public enum CGLStockPriority
    {
        normal = 0,
        high = 1,
    }
}
