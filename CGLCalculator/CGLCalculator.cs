using CutCraftEngineData.DataInput;
using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.CGLCalculator
{
    public class CGLCalculator : ICalculator, ICGLCalculator
    {
        private CutEngine CutEngine;
        
        public CGLCalculator() => 
            this.CutEngine = new CutEngine();

        public string Execute(Command command)
        {
            new CGLCutEngineSetup(command, this.CutEngine);
            return this.CutEngine.Execute();
        }

        public CutEngine GetCutEngine() => this.CutEngine;

        public void OutoputForConsole()
        {
            new GCLConsoleResultReader(this);
        }
    }
}
