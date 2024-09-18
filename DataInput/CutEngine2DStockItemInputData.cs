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
    public class CutEngine2DStockItemInputData
    {
        private readonly IMaterial _material;
        private readonly IStockItem _stockItem;

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
        public CutEngine2DStockItemInputData(List<IMaterial> materialList, IStockItem stockItem, bool allowOverStock = false, StockPriority prioriy = StockPriority.normal)
        {            
            _material = materialList.FirstOrDefault(x => x.id == stockItem.materialId) ?? throw new Exception($"Material with ID: {stockItem.materialId} does not exist in the list of materials to use.");
            _stockItem = stockItem;

            if (!Is2D()) throw new Exception("The material type (kind) has to be 2D");
            if (_material.standardStockItems.Count == 0 && allowOverStock == true) throw new Exception("StandardStockItem is mandatory when the allowOverstock is enabled.");

            aWidth = _stockItem.length;
            aHeight = _stockItem.width;

            thickness = _material.thickness;
            structure = _stockItem.structure;
            aCount = _stockItem.quantity;
            aID = _material.title;
            aWaste = Convert.ToBoolean((int)prioriy);

            sawWidth = _material.kerf;
        }

        /// <summary>
        /// Specifies the type of the material. (mandatory)
        /// Allowed values
        ///     - 1d
        ///         1D materials like profiles, pipes, rods, beams, etc.
        ///     - 2d
        ///         2D materials like glass, panels, fabric, aluminum, cardboard, etc.
        /// </summary>
        public bool Is2D() => _material.kind == "2d" ? true : false;

        /// <summary>
        /// Calculates the real size of the object by subtracting the edging from the length and width.
        /// </summary>
        /// <returns>An anonymous object with the calculated length and width.</returns>
        public dynamic SizeReal() => _stockItem.SizeReal();

        /// <summary>
        /// Edging configuration.
        /// </summary>
        public Edging GetEdging() => _stockItem.edging;
    }
}
