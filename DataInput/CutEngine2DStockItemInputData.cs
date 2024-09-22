using CutCraftEngineData.DataInput;
using CutGLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CutCraftEngineWebSocketCGLService.DataInput
{
    /// <summary>
    /// Get Pieces and StockItems from the command input with specific material.
    /// 
    /// CutGlib has no direct way to add all the different material stocks to the engine.
    /// For each material type, add all stocks and pieces and run the CutGlib calculator. 
    /// After reading the calculations, read the next material and start the calculations again.
    /// </summary>
    /// <remarks>
    /// Names of the fields are the same as in the CutGlib library.
    /// </remarks>
    public class CutEngine2DStockItemInputData
    {
        public readonly List<IPiece> Piece;        
        public readonly List<IStockItem> StockItem;

        public CutEngine2DStockItemInputData(Command command, IMaterial material, StockPriority prioriy = StockPriority.normal)
        {
            StockItem = command.Input.stock.Where(s => s.materialId == material.id).Cast<IStockItem>().ToList() ?? throw new Exception($"Stock with Material ID: {material.id} does not exist in the list of stocks to use.");
            Piece = command.Input.pieces.Where(p => p.materialId == material.id).Cast<IPiece>().ToList() ?? throw new Exception($"Piece with Material ID: {material.id} does not exist in the list of pieces to use.");

            if (!Is2D(material)) throw new Exception("The material type (kind) has to be 2D");
            if (material.standardStockItems.Count == 0 && command.AllowOverstock == true) throw new Exception("StandardStockItem is mandatory when the allowOverstock is enabled.");
        }

        /// <summary>
        /// Specifies the type of the material. (mandatory)
        /// Allowed values
        ///     - 1d
        ///         1D materials like profiles, pipes, rods, beams, etc.
        ///     - 2d
        ///         2D materials like glass, panels, fabric, aluminum, cardboard, etc.
        /// </summary>
        public bool Is2D(IMaterial material) => material.kind == "2d" ? true : false;
    }
}
