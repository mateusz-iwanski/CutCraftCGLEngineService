using CutCraftEngineData.Configuration;
using CutCraftEngineData.DataInput;
using CutCraftEngineWebSocketCGLService.DataInput;
using CutGLib;
using System;
using System.Drawing;

namespace CutCraftEngineWebSocketCGLService.CGLCalculator
{
    public static class CGLPropertiesExtensions
    {
        /// <summary>
        /// Set CutEngine properties.
        /// </summary>
        /// <remarks>
        /// From the Command.Input stocks list, find stock with the same material ID.
        /// By material ID, find the material and get the kerf and deviceId from it.
        /// </remarks>
        /// <param name="cutEngine">This CutEngine</param>
        /// <param name="limits">Command from websocket client</param>
        /// <param name="deviceId">Get from Material->deviceId</param>
        /// <param name="kerf">Get from Material->kerf</param>
        public static void SetProperties(this CutEngine cutEngine, Command command, int deviceId, double kerf)
        {

            var profile = command.Configuration.profiles.FirstOrDefault(p => p.active == true);
            var dataInput = command.Input;

            IStockItem stockItem = dataInput.stock.First(); //, command.Configuration, command.Configuration.profiles.FirstOrDefault(p => p.active == true), SwitchToSmall, command.Configuration.Device
            Limits limits = profile.limits;
            SwitchToSmall switchToSmall = profile.stockOrder.order.switchToSmall;
            IDevice device = dataInput.devices.Find(d => d.id == deviceId);

            /// <summary>
            /// Indicates that all of the parts have to be placed to accept the calculation was successful. If only some of the parts 
            /// are required to be placed then set CompleteMode to false.
            /// This property plays important role when the stock size is less than size of the parts to be cut from the stock.
            /// </summary>
            cutEngine.CompleteMode = true;

            /// <summary>
            /// Defines the saw thickness / kerf size / part to part gap. This value will be taken in account during the calculation.
            /// </summary>
            cutEngine.SawWidth = kerf;


            /// <summary>
            /// Defines the size of unused (trim) size on the left side of the stock. 
            /// </summary>
            cutEngine.TrimLeft = stockItem.edging.left;

            /// <summary>
            /// Defines the size of unused (trim) size on the top side of the stock. 
            /// </summary>
            cutEngine.TrimTop = stockItem.edging.top;

            /// <summary>
            /// Defines the size of unused (trim) size on the right side of the stock. 
            /// </summary>
            cutEngine.TrimRight = stockItem.edging.right;

            /// <summary>
            /// Defines the size of unused (trim) size on the bottom side of the stock. 
            /// </summary>
            cutEngine.TrimBottom = stockItem.edging.bottom;

            /// <summary>
            /// If this property is TRUE then the calculation engine tries to minimize the number of different cutting layouts. This is 
            /// a very important for wood cutting when the operator can load several stocks into the cutting machine and process
            /// them at once.If this property is FALSE(default) then the engine tries to minimize the number of stocks.
            /// </summary>
            cutEngine.UseLayoutMinimization = device.MaxLayoutSize > 1 ? true : false;

            /// <summary>
            /// Defines the maximum number of stocks that can be cut at once from one layout. Works only if 
            /// UseLayoutMinimization is true.
            /// </summary>
            cutEngine.MaxLayoutSize = device.MaxLayoutSize <= 1 ? 1 : device.MaxLayoutSize;


            /// <summary>
            /// Minimal acceptable size of the waste parts (0 - no restrictions). It plays an important role when cut glass stocks – 
            /// because it’s impossible to cut tiny pieces of glass.
            /// </summary>
            cutEngine.WasteSizeMin = switchToSmall.enabled == true ? switchToSmall.minWaste : 0;

            /// <summary>
            /// It produces cutting layouts that require less stock panel rotations during the cutting operations. Therefore it 
            /// minimizes the physical efforts and preparation time during loading / unloading stage of the cutting jobs.
            /// However, it may produce more waste parts and increase the total cutting lengths.
            /// </summary>
            /// 
            cutEngine.MinimizeSheetRotation = !limits.maxCombinations.enabled;

            cutEngine.MaxCutLevel = MaxCutDepth(device);

            
        }

        /// <summary>
        /// Defines how complex the result layout will be. It goes from 2 to 6. Level 2 allows only two cutting planes (so-called
        /// X / Y cuts) and it produces the result with the most material waste. 
        /// However, this level is the simplest one to cut by hand.
        /// </summary>
        public static int MaxCutDepth(IDevice device) =>
            device.maxCutDepth.limit > 6 ? 6 :
            device.maxCutDepth.limit < 2 ? 2 :
            device.maxCutDepth.limit;
    }
}
