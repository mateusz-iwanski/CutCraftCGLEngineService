using CutCraftEngineData.DefaultUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.DataOutput
{
    public class CGLDefaultUnitsFactory : ICGLDataOutputFactory<DefaultUnits>
    {
        /// <summary>
        /// Create default units
        /// 
        /// CutGlib always uses the same default units, so we can hardcode them.
        /// </summary>
        /// <return>DefaulUnits in the list. There is always only one object in the list</return>
        public CGLDefaultUnitsFactory() 
        {
            return;
        }

        public List<DefaultUnits> Get() =>
            new List<DefaultUnits>
            {
                { 
                    new DefaultUnits(
                        _length: "mm",
                        _field: "sqmm",
                        _angle: "deg"
                        )
                }
            };
        
    }
}
