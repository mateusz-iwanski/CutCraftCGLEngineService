using CutCraftEngineData.DataInput;
using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        /// aWaste is true then such stock will be used first before any actual stocks that have aWaste as false. 
        /// </summary>
        public bool aWaste { get; private set; }

        public double sawWidth { get; private set; }


        public CutEngine2DStockItemInputData(IMaterial material, IStockItem stockItem, StockPriority prioriy = StockPriority.normal)
        {
            _material = material;
            _stockItem = stockItem;

            if (!Is2D()) throw new Exception("The material (kind) has to be 2D");

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

        public Edging GetEdging() => _stockItem.edging;
    }
}
