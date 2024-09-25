using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataOutput
{
    /// <summary>
    /// Cut from CutGLib results
    /// </summary>
    public class CGLCut
    {
        /// <summary>
        /// Cut number
        /// </summary>
        public int Cut { get; init; }

        /// <summary>
        /// X axis start point
        /// </summary>
        public double X1 { get; init; }

        /// <summary>
        /// Y axis start point
        /// </summary>
        public double Y1 { get; init; }

        /// <summary>
        /// X axis end point
        /// </summary>
        public double X2 { get; init; }

        /// <summary>
        /// Y axis end point
        /// </summary>
        public double Y2 { get; init; }

        /// <summary>
        /// Sheet on which the cut is placed
        /// </summary>
        public CGLSheet Sheet { get; init; }

        /// <summary>
        /// Layout on which the cut is placed
        /// </summary>
        public CGLLayout Layout { get; init; }

        /// <summary>
        /// Is this a trim cut
        /// </summary>
        public bool Trim { get; init; }

        /// <summary>
        /// Calculate the distance between two points on axis
        /// </summary>
        /// <returns>
        /// Distance between two points in mm
        /// </returns>
        public static double calculateDistance(double X1, double Y1, double X2, double Y2)
        {
            return Math.Sqrt(Math.Pow(X2 - X1, 2) + Math.Pow(Y2 - Y1, 2));
        }
    }
}
