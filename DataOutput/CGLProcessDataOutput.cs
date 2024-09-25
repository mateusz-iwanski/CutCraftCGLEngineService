using CutCraftEngineData.DataInput;
using CutCraftEngineData.DataOutput;
using CutCraftEngineData.DefaultUnits;
using CutCraftEngineWebSocketCGLService.CGLCalculator;
using CutCraftEngineWebSocketCGLService.DataInput;
using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataOutput
{
    public class CGLProcessDataOutput
    {
        private CutEngine _cutEngine { get; set; }
        private readonly Command _command;
        private readonly DataOutputs _dataOutputs;

        public CGLProcessDataOutput(Command command)
        {
            _command = command;

            _dataOutputs = new DataOutputs();

            _dataOutputs.cuttings = new List<Cutting>();

            return;
        }

        public void SetCutEngine(CutEngineInitializer calculator)
        {
            if (calculator.IsExecuted == false)
                throw new InvalidOperationException("Calculator must be executed before processing data.");

            _cutEngine = calculator.GetCutEngine();
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
        //    return new Statistics()
        //    {
        //        _1d = new Statistics1d(),
        //        _2d = new Statistics2d(
        //            field: _cutEngine.UsedStockCount,
        //            usedField: ,
        //            wasteField:,
        //            unusedField:,
        //            cutCount:,
        //            cutsLength:
        //            )
        //    };
        //}

        //private void BuildCuttings(ICutEngine2DStockSetup stock, )
        //{
        //    _dataOutputs.cuttings.Add(
        //        new Cutting()
        //        {
        //            stockItemId = stock.id,
        //            quantity = _cutEngine.UsedStockCount,
        //            pieces = new List<Piece>()
        //        }
        //    );
        //}

        public string BuildVersion()
        {
            return _cutEngine.Version;
        }

    }
}
