using CutCraftEngineData.DataInput;
using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.CGLCalculator
{
    public class CGLCalculator : ICalculator
    {
        private readonly CutEngine _cutEngine;
        
        public CGLCalculator() => 
            _cutEngine = new CutEngine();

        public string Execute(Command command)
        {
            new CGLCutEngineSetup(command, _cutEngine);
            return _cutEngine.Execute();
        }
    }
}
