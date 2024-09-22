using CutCraftEngineData.DataInput;
using CutCraftEngineData.DataOutput;
using CutCraftEngineWebSocketCGLService.DataInput;
using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.CGLCalculator
{
    public class CutEngineInitializer
    {
        private readonly Command _command;
        private CutEngine _cutEngine { get; set; }

        // Add a private field to track the execution status
        private bool _isExecuted;

        // Add a public property to access the execution status
        public bool IsExecuted => _isExecuted;

        // Define an event that gets triggered when _cutEngine.Execute() is called
        public event EventHandler CutEngineExecuted;

        public CutEngineInitializer(Command command)
        {
            _command = command;            
            return;
        }

        public CutEngine GetCutEngine() => _cutEngine;

        public List<DataOutputs> Execute()
        {
            List<DataOutputs> result = new List<DataOutputs>();
            var materials = _command.Input.materials;

            foreach (var material in materials)
            {
                _cutEngine = new CutEngine();

                _cutEngine.SetProperties(_command, material.deviceId, material.kerf);

                var cutEngineInputData = new CutEngine2DStockItemInputData(_command, material);

                foreach (var stockItem in cutEngineInputData.StockItem)
                {
                    var priority = getStockPriority(stockItem.priority);

                    _cutEngine.AddStock(
                        AWidth: stockItem.length,
                        AHeight: stockItem.width,
                        aCount: stockItem.quantity,
                        aID: material.title,
                        aWaste: Convert.ToBoolean((int)priority)
                        );
                }

                foreach (var piece in cutEngineInputData.Piece)
                {
                    _cutEngine.AddPart(
                        AWidth: piece.width,
                        AHeight: piece.length,
                        aCount: piece.quantity,
                        ARotatable: piece.Rotated(),
                        aID: piece.identifier
                        );
                }

                _cutEngine.Execute();                
                _isExecuted = true;
                OnCutEngineExecuted(EventArgs.Empty);
                result.Append(ProcessData());
            }

            return result;
        }

        /// <summary>
        /// After execution, data is processed.
        /// </summary>
        /// <returns>Completed DataOutputs object</returns>
        private DataOutputs ProcessData()
        {
            return new CGLProcessDataOutput(this, _command).BuildDataOutput();
        }

        /// <summary>
        /// If priority is normal then such stock will be used first before any actual stocks that have low priority. 
        /// </summary>
        private StockPriority getStockPriority(string priority) => priority == "normal" ? StockPriority.high : StockPriority.normal;

        // Method to trigger the CutEngineExecuted event
        protected virtual void OnCutEngineExecuted(EventArgs e)
        {
            CutEngineExecuted?.Invoke(this, e);
        }
    }
}
