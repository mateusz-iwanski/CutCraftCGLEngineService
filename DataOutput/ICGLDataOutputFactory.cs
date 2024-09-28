using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.DataOutput
{
    public interface ICGLDataOutputFactory<T>
    {
        public List<T> Get();
    }
}
