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
    /// Data Input from web socket client usable for CutGlib calculator
    /// </summary>
    /// <remarks>
    /// Names of the fields are the same as in the CutGlib library.
    /// </remarks>
    public class CutEngine2DStockItemInputData
    {
        public readonly IPiece Piece;
        public readonly IMaterial Material;
        public readonly IStockItem StockItem;

        /// <summary>
        /// Width (length) of the StockItem.
        /// Mandatory
        /// </summary>
        public double aWidth { get; private set; }
        
        /// <summary>
        /// Heigth (width) of the StockItem.
        /// </summary>
        public double aHeight { get; private set; }

        /// <summary>
        /// Specifies the thickness of the material.
        /// </summary>
        public int thickness { get; private set; }

        /// <summary>
        /// Specifies the structure of the StockItem.
        /// Allowed values:
        ///     - none
        ///         The material has no structure.
        ///     - byLength
        ///         The structure is horizontal (along the length).
        ///    - byWidth
        ///         The structure is vertical(along the width).
        /// </summary>
        public string structure { get; private set; }

        /// <summary>
        /// Specifies quantity of the stock items.
        /// </summary>
        public int aCount { get; private set; }

        /// <summary>
        /// Title of the material group. 
        /// </summary>
        public string aID { get; private set; }

        /// <summary>
        /// aWaste defines if the stock is a waste / leftover from 
        /// previous cutting jobs. If aWaste is true then such stock will be used first before any actual stocks that have aWaste as false. 
        /// </summary>
        public bool aWaste { get; private set; }

        public double sawWidth { get; private set; }

        /// <summary>
        /// Input data with the specified stock with items.
        /// </summary>
        /// <remarks>
        /// A single stock can only have one type of material with specific quantity.
        /// </remarks>
        /// <param name="materialList">All materials to use</param>
        /// <param name="stockItem">StockItem</param>
        /// <param name="allowOverStock">
        ///     The allowOverstock parameter controls the behavior of the cutting optimization algorithm.
        ///     When set to true, the algorithm is permitted to allocate more stock items than currently available.
        /// 
        ///     This can be useful in scenarios where future stock replenishment is anticipated or over-allocation is acceptable.
        ///     Please note that enabling this option may lead to situations where demand exceeds supply.
        ///     Note that it applies only to standard stock sizes.So, in order to use more stock items than available, 
        ///     you have to define standard stock items in material definition.
        ///     Then, you have to add stock item with the same standard size.
        /// </param>
        /// <param name="prioriy">StockPriority - If is high then such stock will be used first before any actual stocks that have priority as normal. </param>
        /// <exception cref="Exception">Exception - when the material is not a 2D type (kind)</exception>
        public CutEngine2DStockItemInputData(Command command, IMaterial material, StockPriority prioriy = StockPriority.normal)
        {
            Material = material;//command.Input.materials.FirstOrDefault(x => x.id == stockItem.materialId) ?? throw new Exception($"Material with ID: {stockItem.materialId} does not exist in the list of materials to use.");
            StockItem = command.Input.stock.FirstOrDefault(s => s.materialId == Material.id) ?? throw new Exception($"Stock with Material ID: {Material.id} does not exist in the list of stocks to use.");
            Piece = command.Input.pieces.FirstOrDefault(p => p.materialId == Material.id) ?? throw new Exception($"Piece with Material ID: {Material.id} does not exist in the list of pieces to use.");

            if (!Is2D()) throw new Exception("The material type (kind) has to be 2D");
            if (Material.standardStockItems.Count == 0 && command.AllowOverstock == true) throw new Exception("StandardStockItem is mandatory when the allowOverstock is enabled.");

            aWidth = StockItem.length;
            aHeight = StockItem.width;

            thickness = Material.thickness;
            structure = StockItem.structure;
            aCount = StockItem.quantity;
            aID = Material.title;
            aWaste = Convert.ToBoolean((int)prioriy);

            sawWidth = Material.kerf;
        }

        /// <summary>
        /// Specifies the type of the material. (mandatory)
        /// Allowed values
        ///     - 1d
        ///         1D materials like profiles, pipes, rods, beams, etc.
        ///     - 2d
        ///         2D materials like glass, panels, fabric, aluminum, cardboard, etc.
        /// </summary>
        public bool Is2D() => Material.kind == "2d" ? true : false;

        /// <summary>
        /// Calculates the real size of the object by subtracting the edging from the length and width.
        /// </summary>
        /// <returns>An anonymous object with the calculated length and width.</returns>
        public dynamic SizeReal() => StockItem.SizeReal();

        /// <summary>
        /// Edging configuration.
        /// </summary>
        public Edging GetEdging() => StockItem.edging;
    }
}
