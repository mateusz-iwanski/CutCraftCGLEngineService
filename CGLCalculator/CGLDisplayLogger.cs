using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CutCraftCGLEngineService.CGLCalculator.CGLResultPrinter;

namespace CutCraftCGLEngineService.CGLCalculator
{
    /// <summary>
    /// Disply logger for CutGlib engine by CutGLib format.
    /// 
    /// It is used to print the result to the console/file/e.g whenever Execute is called by CutEngine.
    /// </summary>
    /// <remarks>
    /// CGLDisplayLogger has to be initialize before CGLCalculator is executed.
    /// </remarks>
    public class CGLDisplayLogger
    {
        private readonly CGLCalculator _calculator;
        private readonly Action<string> _printer;

        public CGLDisplayLogger(CGLCalculator calculator, Action<string> print)
        {
            _calculator = calculator;

            _printer = print ?? throw new ArgumentNullException(nameof(print)); ;
            
            // Subscribe to the CutEngineInitializerExecuted event. It's raise when CutEngine.Execute() is called.
            _calculator.InitializeExecutedEvent += Display;
        }

        private void Display(object sender, EventArgs e)
        {
            new CGLResultPrinter(_calculator, _printer);
        }
    }
}
