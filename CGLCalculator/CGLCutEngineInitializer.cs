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
    /// <summary>
    /// Execute CutEngine calculator and get the list of cuttings 
    /// data group for each material from calculator result.
    /// 
    /// Every material will have its own list of cuttings data group.
    /// Everytime it's new material to calculate, it will create new instance of CutEngine.
    /// </summary>
    public class CGLCutEngineInitializer
    {
        private readonly Command _command;
        private CutEngine _cutEngine { get; set; }

        // Add a private field to track the execution status
        private bool _isExecuted;

        // Add a public property to access the execution status
        public bool IsExecuted => _isExecuted;

        // Define an event that gets triggered when _cutEngine.Execute() is called
        public event EventHandler CutEngineExecuted;

        public CGLCutEngineInitializer(Command command)
        {            
            _command = command;
            return;
        }

        public CutEngine GetCutEngine() => _cutEngine;

        /// <summary>
        /// CutGlib engine can only be execute once on each material,
        /// this is how CutGLib engine works by default.
        /// 
        /// If there are multiple materials in the input, this method will run CutEngine for each material,
        /// and add the list of cuttings data group of material to the global list of cuttings for each material.
        /// </summary>
        /// <returns></returns>
        public List<List<Cutting>> Execute()
        {
            List<List<Cutting>> result = new List<List<Cutting>>();
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
                        aID: stock.id.ToString(),  // save stock id as aID in new stock
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
                OnCutEngineExecuted(new CGLCutEngineEventArgs(_cutEngine));

                result.Add(new CGLCuttingsFactory(_cutEngine).Get());
            }

            return result;
        }


        /// <summary>
        /// When finished executing the CutEngine, raise the CutEngineExecuted event
        /// 
        /// When it's more than one material, CutEngine will be executed for each material.
        /// Before start new execution, CutEngine calculator data can be retrieve before start a new instance.
        /// </summary>
        protected virtual void OnCutEngineExecuted(CGLCutEngineEventArgs e)
        {
            CutEngineExecuted?.Invoke(this, e);
        }
    }
}
