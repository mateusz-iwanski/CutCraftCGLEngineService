using CutCraftEngineData.DataOutput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataOutput
{
    /// <summary>
    /// Mapping list of CutGLib CGLPart to list of DataOutput CuttingPiece
    /// </summary>
    public class CGLPiecesMapper
    {
        List<CGLPart> _parts;

        public CGLPiecesMapper(List<CGLPart> parts)
        {
            _parts = parts;
            return;
        }

        public List<CuttingPiece> Map()
        {
            var pieces = new List<CuttingPiece>();

            foreach (var part in _parts)
            {
                pieces.Add(new CuttingPiece()
                    {
                        pieceId = part.Id,
                        x = part.X,
                        y = part.Y,
                        rotated = part.Rotated,
                        mirrored = false
                    });
            }

            return pieces;
        }
    }
}
