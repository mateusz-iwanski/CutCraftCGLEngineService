using CutCraftEngineData.DataInput;
using CutCraftEngineData.DataOutput;
using CutCraftEngineWebSocketCGLService.DataInput;
using CutCraftEngineWebSocketCGLService.DataOutput;
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

        private readonly CGLProcessDataOutput _processDataOutput;

        // Add a public property to access the execution status
        public bool IsExecuted => _isExecuted;

        // Define an event that gets triggered when _cutEngine.Execute() is called
        public event EventHandler CutEngineExecuted;

        public CutEngineInitializer(Command command)
        {            
            _command = command;
            _processDataOutput = new CGLProcessDataOutput(_command);
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

                foreach (var stock in cutEngineInputData.Stock)
                {
                    _cutEngine.AddStock(
                        AWidth: stock.aWidth,
                        AHeight: stock.aHeight,
                        aCount: stock.aCount,
                        aID: stock.aID,
                        aWaste: stock.aWaste
                        );
                }

                foreach (var piece in cutEngineInputData.Piece)
                {
                    _cutEngine.AddPart(
                        AWidth: piece.aWidth,
                        AHeight: piece.aHeight,
                        aCount: piece.aCount,
                        ARotatable: piece.aRotatable,
                        aID: piece.aID
                        );
                }

                _cutEngine.Execute();                
                _isExecuted = true;
                OnCutEngineExecuted(EventArgs.Empty);


                CuttingsMapper c = new CuttingsMapper(_cutEngine);
                var a = Newtonsoft.Json.JsonConvert.SerializeObject(c.Map());
                Console.WriteLine(a);

                //result.Append(processData());
            }

            return result;
        }

        /// <summary>
        /// After execution, data is processed.
        /// </summary>
        /// <returns>Completed DataOutputs object</returns>
        //private DataOutputs processCuttingOutputData(ICutEngine2DStockSetup stock)
        //{
        //    _processDataOutput.SetCutEngine(this);
        //    _processDataOutput.
        //}

        // Method to trigger the CutEngineExecuted event
        protected virtual void OnCutEngineExecuted(EventArgs e)
        {
            CutEngineExecuted?.Invoke(this, e);
        }
    }
}
