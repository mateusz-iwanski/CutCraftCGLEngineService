using CutCraftEngineData.DataInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataInput
{
    /// <summary>
    /// Check the piece structure state based on the data input from websocket client
    /// </summary>
    internal static class PieceStructureState
    {
        private static readonly Dictionary<string, bool> States = new Dictionary<string, bool>
        {
            { "none,byLength,byWidth", false },  // can be rotated
            { "none,byWidth,byLength", false },  // can be rotated

            { "byLength,none,byWidth", false },  // can be rotated
            { "byLength,byWidth,none", false },  // can be rotated

            { "byWidth,byLength,none", false },  // can be rotated
            { "byWidth,none,byLength", false },  // can be rotated

            { "byLength", true },  // structure by length - can't be rotated
            { "byWidth", true }  // structure by width - can't be rotated
        };

        /// <summary>
        /// Check if the piece can be rotated by Piece.structure
        /// </summary>
        /// <remarks>
        /// If data input piece contains structure:
        ///     - "none,byLength,byWidth" return true ( can be rotated )
        ///     - "byLength" return false ( can't be rotated )
        ///     - "byWidth" return false ( can't be rotated )
        /// </remarks>
        /// <param name="state">Available - "none,byLength,byWidth" , "byLength" , "byWidth"</param>
        /// <returns>Bool or throw ArgumentNullException if not contains</returns>
        public static bool Rotated(this IPiece piece)
        {
            if (!States.ContainsKey(piece.structure))
            {
                throw new ArgumentNullException(nameof(piece.structure));
            }
            return !States[piece.structure];
        }
    }
}
