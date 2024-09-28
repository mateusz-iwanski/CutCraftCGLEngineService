using CutCraftEngineData.DataOutput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.DataOutput
{
    public class CGLGlobalStatisticsFactory : ICGLDataOutputFactory<Statistics>
    {
        private readonly List<Cutting> _cuttings;
        private Statistics2d _statistics2D { get; set; } 

        public CGLGlobalStatisticsFactory(List<Cutting> cuttings)
        {
            _cuttings = cuttings;
            return;
        }

        private void build()
        {
            Statistics2d statistics2D = new Statistics2d();

            foreach (var cuttings in _cuttings)
            {
                statistics2D.field += cuttings.statistics._2d.field;
                statistics2D.usedField += cuttings.statistics._2d.usedField;
                statistics2D.wasteField += cuttings.statistics._2d.wasteField;
                statistics2D.unusedField += cuttings.statistics._2d.unusedField;
                statistics2D.cutCount += cuttings.statistics._2d.cutCount;
                statistics2D.cutsLength += cuttings.statistics._2d.cutsLength;
            }
        }

        /// <summary>
        /// Return the global statistics
        /// 
        /// Always return just one global statistics object in list.
        /// </summary>
        /// <returns></returns>
        public List<Statistics> Get()
        {
            return new List<Statistics>()
            {
                {
                    new Statistics(){
                        _1d = new Statistics1d(),
                        _2d = _statistics2D
                    }
                }
            };
        }
    }
}
