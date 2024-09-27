using CutCraftEngineData.DataInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataInput
{
    /// <summary>
    /// Unify the Stock data from the input data before adding it to the CutGLib engine.
    /// </summary>
    public class CutEngine2DStockSetup
    {
        private readonly IStockItem _stock;

        public int id { get; set; }

        /// <summary>
        /// Width (length) of the StockItem.
        /// </summary>
        public double aWidth { get; set; }

        /// <summary>
        /// Height of the StockItem.
        /// </summary>
        public double aHeight { get; set; }

        /// <summary>
        /// Specifies quantity of the stock items.
        /// </summary>
        public int aCount { get; set; }

        /// <summary>
        /// The user defined identifier of the StockItem. It must be Code128 barcode compatible.
        /// </summary>
        public string aID { get; set; }

        /// <summary>
        /// If aWaste is true then such stock will be used first before any actual 
        /// stocks that have aWaste as false.
        /// </summary>
        public bool aWaste { get; set; }

        public CutEngine2DStockSetup(IStockItem stock)
        {
            _stock = stock;

            id = stock.id;
            aWidth = stock.length;
            aHeight = stock.width;
            aCount = stock.quantity;
            aID = stock.identifier;
            aWaste = Convert.ToBoolean(
                (int)getStockPriority(stock.priority)
                );
        }

        /// <summary>
        /// If priority is normal then such stock will be used first before any actual stocks that have low priority. 
        /// </summary>
        private CGLStockPriority getStockPriority(string priority) => priority == "normal" ? CGLStockPriority.high : CGLStockPriority.normal;
    }

}
