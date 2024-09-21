using CutCraftEngineData.DataInput;
using CutCraftEngineWebSocketCGLService.DataInput;
using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.CGLCalculator
{
    internal class CGLCutEngineSetup
    {

        public CGLCutEngineSetup(Command command, CutEngine cutEngine)
        {
            var stockList = command.Input.stock;

            foreach (var stock in stockList)
            {
                var materials = command.Input.materials;

                foreach (var material in materials)
                {
                    cutEngine.SetProperties(command, material.deviceId, material.kerf);

                    var cutEngineInputData = new CutEngine2DStockItemInputData(command, material, StockPriority.normal);

                    cutEngine.AddStock(
                        cutEngineInputData.aWidth,
                        cutEngineInputData.aHeight,
                        cutEngineInputData.aCount,
                        cutEngineInputData.aID,
                        cutEngineInputData.aWaste
                        );

                    cutEngine.AddPart(
                        cutEngineInputData.Piece.width,
                        cutEngineInputData.Piece.length,
                        cutEngineInputData.Piece.quantity,
                        cutEngineInputData.Piece.Rotated(),
                        cutEngineInputData.Piece.identifier
                        );
                }
            }
        }       
    }
}
