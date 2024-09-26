using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftEngineWebSocketCGLService.DataExporter
{
    /// <summary>
    /// Export data to a file
    /// </summary>
    public class FileExporter
    {
        private readonly IDataFileExporter _dataExporter;

        public FileExporter(IDataFileExporter dataExporter)
        {
            _dataExporter = dataExporter;
        }

        public void Export()
        {
            _dataExporter.Export();
        }
    }
}
