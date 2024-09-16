using CutCraftEngineData.DataInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataInput
{
    public class CutEngine2DPieceInputData
    {
        public readonly IPiece _piece;
        public readonly CutEngine2DStockItemInputData _stock;

        /// <summary>
        /// Width (length) of the piece.
        /// Mandatory
        /// </summary>
        public double aWidth { get; private set; }

        /// <summary>
        /// Heigth (width) of the piece.
        /// </summary>
        public double aHeight { get; private set; }

        /// <summary>
        /// Specifies quantity of the pieces.
        /// </summary>
        public int aCount { get; private set; }

        /// <summary>
        /// Specifies if the piece can be rotated.
        /// </summary>
        public bool aRotatable { get; private set; }

        /// <summary>
        /// Title of the material group. 
        /// </summary>
        public string aID { get; private set; }

        public CutEngine2DPieceInputData(CutEngine2DStockItemInputData stock, IPiece piece)
        {
            _piece = piece;
            _stock = stock;

            aWidth = Surplus(_piece.length);
            aHeight = Surplus(_piece.width);
            aCount = _piece.quantity;
            aRotatable = Rotatable();
            aID = _piece.identifier;
        }

        /// <summary>
        /// This setting defines the default surplus for pieces, meaning the amount by which each cut element will be enlarged on all sides. 
        /// 
        /// For example:
        /// if piece is 300x200 and the surplus is set to 2, the optimizer will use a size of 304x204. 
        /// In the case of 1D materials, this applies, of course, only to the length.
        /// </summary>
        public int Surplus(int dimension) => dimension + (_piece.surplus * 2);

        /// <summary>
        /// Calculates the size of the _piece based on length, width, surplus and Margin.
        /// </summary>
        /// <returns>Returns an anonymous object with the calculated length and width.</returns>
        public dynamic SizeReal() => _piece.SizeReal();

        /// <summary>
        /// Specifies acceptable structures of the _piece. Allowed values: none, byLength, byWidth.
        /// Allowed values
        ///     - none
        ///         The material has no structure. Is not good idea to use it, if set none optimalization not rotate the _piece.
        ///         Is better to use
        ///     - byLength
        ///         The structure is horizontal (along length).
        ///     - byWidth
        ///         The structure is vertical (along width).
        ///     - "none,byLength,byWidth" = The material has no structure or with structure but can be rotate.
        ///       With this settings the _piece will can be rotated by optimalization.
        ///     - any
        public bool Rotatable() => _piece.structure == "byLength" || _piece.structure == "byWidth" ? false : true;  
    }
}
