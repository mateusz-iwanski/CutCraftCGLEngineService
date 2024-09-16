using CutCraftEngineData.DataInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataInput
{
    public class CutEngineStockTrim
    {
        public readonly IStockItem _stockItem;

        public CutEngineStockTrim(IStockItem stockItem)
        {
            this._stockItem = stockItem;
        }
    }
}
