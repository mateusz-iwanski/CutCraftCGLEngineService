using CutCraftEngineData.DataOutput;
using CutCraftEngineData.DefaultUnits;
using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.DataOutput
{
    /// <summary>
    /// Build CutCraftEngineData.DataOutputs
    /// 
    /// DataOutputs is response for Client calculating request.
    /// </summary>
    public class CGLDataOutputFactory : ICGLDataOutputFactory<DataOutputs>
    {
        private readonly CutEngine _cutEngine;

        private DataOutputs _dataOutputs { get; set; }

        private readonly CutCraftEngineData.DataInput.DataInput _dataInput;

        private Statistics _statistics { get; set; }

        // list of cuttings is a result from CGLCutEngineInitializer.Execute()
        private List<Cutting> _cuttings { get; set; }

        public CGLDataOutputFactory(CutEngine cutEngine, CutCraftEngineData.DataInput.DataInput dataInput, List<List<Cutting>> cuttings)
        {
            _cutEngine = cutEngine;
            _dataInput = dataInput;
            _cuttings = standarizeCuttings(cuttings);

            // set basic data
            _dataOutputs = new DataOutputs();
            _dataOutputs.version = cutEngine.Version;
            _dataOutputs.defaultUnits = new CGLDefaultUnitsFactory().Get().FirstOrDefault();
            _dataOutputs.statistics = BuildGlobalStatistics();
            _dataOutputs.cuttings = _cuttings;
        }

        private List<Cutting> standarizeCuttings(List<List<Cutting>> cuttings) 
        { 
            List<Cutting> standarizeCuttings = new List<Cutting>();

            foreach (var cuts in cuttings)
            {
                foreach (var cut in cuts)
                    standarizeCuttings.Add(cut);
            }

            return standarizeCuttings;
        }

        private Statistics BuildGlobalStatistics()
        {
            Statistics2d statistics2D = new Statistics2d();

            foreach(var cuttings in _cuttings)
            {
                statistics2D.field += cuttings.statistics._2d.field;
                statistics2D.usedField += cuttings.statistics._2d.usedField;
                statistics2D.wasteField += cuttings.statistics._2d.wasteField;
                statistics2D.unusedField += cuttings.statistics._2d.unusedField;
                statistics2D.cutCount += cuttings.statistics._2d.cutCount;
                statistics2D.cutsLength += cuttings.statistics._2d.cutsLength;
            }

            return new Statistics()
            {
                _1d = new Statistics1d(),
                _2d = statistics2D
            };

        }   

        public List<DataOutputs> Get() => new List<DataOutputs>() { _dataOutputs };
    }
}
