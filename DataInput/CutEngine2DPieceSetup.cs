using CutCraftEngineData.DataInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataInput
{

    /// <summary>
    /// Unify the Piece data from the input data before adding it to the CutGLib engine.
    /// </summary>
    public class CutEngine2DPieceSetup
    {
        private readonly IPiece _piece;

        public int id { get; private set; }

        /// <summary>
        /// Width (length) of the piece.        
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

        public CutEngine2DPieceSetup(IPiece piece)
        {
            _piece = piece;

            id = _piece.id;
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
        /// if piece is 300x200 and the surplus is set to -2, the optimizer will use a size of 296x296. 
        /// </summary>
        /// <remarks>
        /// If surplus 
        /// </remarks>
        public int Surplus(int dimension) => 
            _piece.surplus > 0 ? dimension + (_piece.surplus * 2) :
            _piece.surplus < 0 ? dimension - (_piece.surplus * 2) :
            dimension;

        /// <summary>
        /// Calculates the size of the Piece based on length, width, surplus and Margin.
        /// </summary>
        /// <returns>Returns an anonymous object with the calculated length and width.</returns>
        public dynamic SizeReal() => _piece.SizeReal();

        /// <summary>
        /// Specifies acceptable structures of the Piece. Allowed values: none, byLength, byWidth.
        /// Allowed values
        ///     - none
        ///         The material has no structure. Is not good idea to use it, if set none optimalization not rotate the Piece.
        ///         Is better to use
        ///     - byLength
        ///         The structure is horizontal (along length).
        ///     - byWidth
        ///         The structure is vertical (along width).
        ///     - "none,byLength,byWidth" = The material has no structure or with structure but can be rotate.
        ///       With this settings the Piece will can be rotated by optimalization.
        ///     - any
        public bool Rotatable() => _piece.structure == "byLength" || _piece.structure == "byWidth" ? false : true;
    }
}
