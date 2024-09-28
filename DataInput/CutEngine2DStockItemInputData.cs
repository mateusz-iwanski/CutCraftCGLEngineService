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

namespace CutCraftCGLEngineService.DataInput
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
        public readonly List<CutEngine2DPieceSetup> Piece;        
        public readonly List<CutEngine2DStockSetup> Stock;

        /// <summary>
        /// Constructor find stock and pieces with specific material.
        /// 
        /// Pieces are unify to CutGLib standard.
        /// </summary>
        /// <param name="command">Command with DataInput</param>
        /// <param name="material">Material to find pieces and stocks</param>
        /// <param name="prioriy"></param>
        /// <exception cref="Exception"></exception>
        public CutEngine2DStockItemInputData(Command command, IMaterial material)
        {
            var stocksFromCutGLib = command.Input.stock.Where(s => s.materialId == material.id).Cast<IStockItem>().ToList() ?? throw new Exception($"Stock with Material ID: {material.id} does not exist in the list of stocks to use.");            
            var pieceFromCutGLib = command.Input.pieces.Where(p => p.materialId == material.id).Cast<IPiece>().ToList() ?? throw new Exception($"Piece with Material ID: {material.id} does not exist in the list of pieces to use.");

            Piece = UnifyPieces(pieceFromCutGLib);
            Stock = UnifyStocks(stocksFromCutGLib);

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

        /// <summary>
        /// Unify the Piece data from the input data before adding it to the CutGLib engine.
        /// </summary>
        /// <param name="pieces">List of pieces from DataInput</param>
        /// <returns>Unify pices in list</returns>
        private List<CutEngine2DPieceSetup> UnifyPieces(List<IPiece> pieces)
        {
            var result = new List<CutEngine2DPieceSetup>();

            foreach (var piece in pieces)
            {
                result.Add(new CutEngine2DPieceSetup(piece));
            }

            return result;
        }

        /// <summary>
        /// Unify the Piece data from the input data before adding it to the CutGLib engine.
        /// </summary>
        /// <param name="stocks">List of stocks from DataInput</param>
        /// <returns>Unify stocks in list</returns>
        private List<CutEngine2DStockSetup> UnifyStocks(List<IStockItem> stocks)
        {
            var result = new List<CutEngine2DStockSetup>();

            foreach (var stock in stocks)
            {
                result.Add(new CutEngine2DStockSetup(stock));
            }

            return result;
        }

    }
}
