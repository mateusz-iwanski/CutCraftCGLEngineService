using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.DataOutput
{
    /// <summary>
    /// Piece/part from CutGLib results 
    /// </summary>
    public class CGLPart
    {
        /// <summary>
        /// Unique id of part from external data input system
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Part id assigned by CutGLib
        /// </summary>
        public int PartId { get; init; }

        /// <summary>
        /// Sheet on which the part is placed
        /// </summary>
        public CGLSheet Sheet { get; init; }

        /// <summary>
        /// X position of the piece.
        /// </summary>
        public double X { get; init; }

        /// <summary>
        /// Y position of the piece.
        /// </summary>
        public double Y { get; init; }

        /// <summary>
        /// Width of the piece.
        /// </summary>
        public double Width { get; init; }

        /// <summary>
        /// Height of the piece.
        /// </summary>
        public double Height { get; init; }

        /// <summary>
        /// If piece is rotated
        /// </summary>
        public bool Rotated { get; init; }
}
}
