using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutCraftCGLEngineService.CGLCalculator
{
    /// <summary>
    /// Console logger for CutGlib engine.
    /// 
    /// It is used to print the result to the console whenever Execute is called by CutEngine.
    /// </summary>
    /// <remarks>
    /// CGLConsoleLogger has to be initialize before CGLCalculator is executed.
    /// </remarks>
    public class CGLConsoleLogger
    {
        private readonly CGLCalculator _calculator;

        public CGLConsoleLogger(CGLCalculator calculator)
        {
            _calculator = calculator;
            
            // Subscribe to the CutEngineInitializerExecuted event. It's raise when CutEngine.Execute() is called.
            _calculator.InitializeExecutedEvent += Display;
        }

        private void Display(object sender, EventArgs e)
        {
            new CGLConsoleResultPrinter(_calculator);
        }
    }
}
