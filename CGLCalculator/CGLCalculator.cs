﻿using CutCraftEngineData.DataInput;
using CutCraftEngineData.DataOutput;
using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.CGLCalculator
{
    /// <summary>
    /// Main CutGlib engine calculator class.
    /// </summary>
    public class CGLCalculator : ICalculator
    {
        private CutEngine _cutEngine { get; set; }
        private Command _command { get; set; }

        /// <summary>
        /// Group of cuttings data for each material
        /// </summary>
        private List<List<Cutting>> _dataCuttingsGroupOutputs { get; set; }
        private CGLCutEngineInitializer _cutEngineInitializer { get; set; }

        /// <summary>
        /// Event raise when CutEngine is executed.
        /// </summary>
        public event EventHandler InitializeExecutedEvent;

        public CGLCalculator() { }

        /// <summary>
        /// Calculate the cutting plan and set results to the DataOutputs.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Completed DataOutputs object</returns>
        public bool Execute(Command command)
        {
            _command = command;
            _cutEngineInitializer = new CGLCutEngineInitializer(_command);

            // Subscribe to the CutEngineExecuted event. It's raise when _cutEngine.Execute() is called.            
            _cutEngineInitializer.CutEngineExecuted += (sender, e) => InitializeExecutedEvent?.Invoke(sender, e);
            _dataCuttingsGroupOutputs = _cutEngineInitializer.Execute();
            
            return true;
        }   

        public bool IsExecuted()
        {
            if (_cutEngineInitializer == null)
                return false;
            else
                return _cutEngineInitializer.IsExecuted;
        }

        public List<List<Cutting>> GetDataCuttingsOutputs() => _dataCuttingsGroupOutputs;

        public CutEngine GetCutEngine() => _cutEngineInitializer.GetCutEngine();

    }
}
