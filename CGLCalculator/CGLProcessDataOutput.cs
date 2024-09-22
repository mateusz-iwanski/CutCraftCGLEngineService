using CutCraftEngineData.DataInput;
using CutCraftEngineData.DataOutput;
using CutCraftEngineData.DefaultUnits;
using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.CGLCalculator
{
    public class CGLProcessDataOutput
    {
        private readonly CutEngine _cutEngine;        
        private readonly Command _command;

        public CGLProcessDataOutput(CutEngineInitializer calculator, Command command)
        {
            if (calculator.IsExecuted == false)
                throw new InvalidOperationException("Calculator must be executed before processing data.");

            _cutEngine = calculator.GetCutEngine();
            _command = command;

            return;
        }

        public DataOutputs BuildDataOutput()
        {
            return new DataOutputs() 
            { 
                version = BuildVersion(),
                defaultUnits = BuildDefaultUnits(),
            };
        }

        /// <summary>
        /// Get the DefaultUnits.
        /// 
        /// CutGlib does not have DefaultUnits in the output, must be retrieved from the input command.
        /// </summary>
        /// <returns></returns>
        private DefaultUnits BuildDefaultUnits()
        {
            DefaultUnitsInput defaultUnitsInput = _command.Input.defaultUnits;

            return new DefaultUnits(
                _angle: defaultUnitsInput.angle,
                _length: defaultUnitsInput.length,
                _field: defaultUnitsInput.field
                );
        }

        //private Statistics BuildStatistics()
        //{
            //return new Statistics()
            //{
            //    _1d = new Statistics1d(),
            //    _2d = new Statistics2d(
            //        field:, 
            //        usedField:, 
            //        wasteField:, 
            //        unusedField:, 
            //        cutCount:, 
            //        cutsLength:
            //        )
            //};
        //}

        private List<Cutting> BuildCuttings()
        {
            return new List<Cutting>();
        }

        public string BuildVersion()
        {
            return _cutEngine.Version;
        }

    }
}
