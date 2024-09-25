using CutCraftEngineData.DataOutput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataOutput
{
    /// <summary>
    /// Mapping list of CutGLib CGLCut to list of DataOutput Cut
    /// </summary>
    public class CutsMapper 
    {
        private readonly List<CGLCut> _cuts;

        public CutsMapper(List<CGLCut> cuts)
        {
            _cuts = cuts;
            return;
        }

        public List<Cut> Map()
        {            
            var cuts = new List<Cut>();
            foreach (var cut in _cuts)
            {
                cuts.Add(new Cut()
                {
                    startX = cut.X1,
                    startY = cut.Y1,
                    endX = cut.X2,
                    endY = cut.Y2
                });
            }
            
            return cuts;
        }
    }
}
