using CutCraftEngineData.DataInput;
using CutCraftEngineData.DataOutput;
using CutGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService
{
    /// <summary>
    /// Interface for all calculators included in the service (CutGlib, TonCut, etc)
    /// </summary>
    public interface ICalculator
    {        
        public bool Execute(Command command);
        public event EventHandler InitializeExecutedEvent;
        public List<DataOutputs> GetDataOutputs();
    }
}
