using CutCraftEngineData.Configuration;
using CutCraftEngineData.DataInput;
using CutCraftEngineWebSocketCGLService.DataInput;
using CutGLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CutCraftEngineWebSocketCGLService
{
    public class Calculator
    {
        private CutEngine _cutEngine;

        public Calculator(CutEngine2DStockItemInputData stockItem, Limits limits, SwitchToSmall switchToSmall, Device device)
        {
            _cutEngine = new CutEngine();

            _cutEngine.TrimLeft = stockItem.GetEdging().left;
            _cutEngine.TrimTop = stockItem.GetEdging().top;
            _cutEngine.TrimRight = stockItem.GetEdging().right;
            _cutEngine.TrimBottom = stockItem.GetEdging().bottom;

            _cutEngine.SawWidth = stockItem.sawWidth;

            _cutEngine.MinimizeSheetRotation = !limits.maxCombinations.enabled;
            _cutEngine.MaxCutLevel = limits.maxCombinations.limit;  //czy przypadkiem z maxCutDepth z Device nie powinno byc?
            

            // if 0 then no limit
            _cutEngine.WasteSizeMin = switchToSmall.minWaste;

            /// <summary>
            /// Defines the maximum number of stocks that can be cut at once from one layout. Works only if 
            /// UseLayoutMinimization is true.
            /// </summary>
            _cutEngine.MaxLayoutSize = device.MaxLayoutSize <= 1 ? 1 : device.MaxLayoutSize;

            /// <summary>
            /// If this property is TRUE then the calculation engine tries to minimize the number of different cutting layouts. This is 
            /// a very important for wood cutting when the operator can load several stocks into the cutting machine and process
            /// them at once.If this property is FALSE(default) then the engine tries to minimize the number of stocks.
            /// </summary>
            _cutEngine.UseLayoutMinimization = device.MaxLayoutSize > 1 ? true : false;

        }
    }
}
