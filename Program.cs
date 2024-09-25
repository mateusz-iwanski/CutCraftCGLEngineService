using CutGLib;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CommandLine;
using Newtonsoft.Json.Linq;
using CutCraftEngineData.Configuration;
using static System.Formats.Asn1.AsnWriter;
using System;
using CutCraftEngineData.DataInput;
using CutCraftEngineWebSocketCGLService.CGLCalculator;

namespace CutCraftEngineWebSocketCGLService
{
    internal class Program
    {
        public class Options
        {
            [
                Option(
                    'c', "cutter_engine",
                    Required = true,
                    HelpText = "Set the cutter engine to be used. Available engines: cutglib, toncut, auto"
                )
            ]
            public string CutterEngineSelection { get; set; }
        }

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var serviceProvider = host.Services;

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            // Parse the command-line arguments
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts, serviceProvider))
                .WithNotParsed<Options>((errs) => HandleParseError(errs));
        }

        // Handle options
        static void RunOptionsAndReturnExitCode(Options opts, IServiceProvider serviceProvider)
        {
            CutterEngine cutterEngine;

            // check if the cutter engine name from commandline is valid
            if (!Enum.TryParse<CutterEngine>(opts.CutterEngineSelection, out cutterEngine))
            {
                Console.WriteLine($"Invalid cutter engine: {opts.CutterEngineSelection}. " +
                    $"Use one of - {string.Join(',', Enum.GetNames(typeof(CutterEngine)))}"
                    );

                throw new ArgumentException();
            }

            using var scope = serviceProvider.CreateScope();

            ICalculator engineCalculator = scope.ServiceProvider.GetRequiredService<Func<string, ICalculator>>()(cutterEngine.ToString());

            // engineCalculator.processData(command);


            // Deserialize the JSON file into a Command object
            string json = File.ReadAllText("test_input.json");
            Command command = JsonConvert.DeserializeObject<Command>(json);

            engineCalculator.Execute(command);

            Console.WriteLine("================" + engineCalculator.GetDataOutputs());


            // here we can connect to OnCutEngineExecuted event
            engineCalculator.PrintResultToConsole();

        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            // Handle errors
            Console.WriteLine("Error parsing arguments.");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
               .ConfigureServices((_, services) =>
               {
                   var startup = new Startup();
                   startup.ConfigureServices(services);
               });


        private static void OutputSheetResults_by_Parts(CutEngine aCalculator)
        {
            int StockNo, iCut, iPart;
            long CutsCount;
            double Width, Height, X1 = 0, Y1 = 0, X2 = 0, Y2 = 0;
            bool active;
            string id;
            Console.Write("\n");
            Console.Write("OUTPUT CUTS RESULTS\n");
            Console.Write("Used {0} sheets\n", aCalculator.UsedStockCount);
            // Output guilltoine cuts for each sheet
            for (StockNo = 0; StockNo < aCalculator.StockCount; StockNo++)
            {
                aCalculator.GetStockInfo(StockNo, out Width, out Height, out active);
                // Sheet was not used during calculation
                if (!active)
                {
                    Console.Write("Sheet={0} was not used.\n", StockNo);
                    continue;
                }
                Console.Write("Sheet={0}: Width={1} Height={2}\n", StockNo, Width, Height);
                // First output any trim cuts for the sheet StockNo
                CutsCount = aCalculator.GetStockTrimCutCount(StockNo);
                for (iCut = 0; iCut < CutsCount; iCut++)
                {
                    aCalculator.GetStockTrimCut(StockNo, iCut, out X1, out Y1, out X2, out Y2);
                    Console.Write("Trim  Cut={0}:  X1={1};  Y1={2};  X2={3};  Y2={4}\n", iCut, X1, Y1, X2, Y2);
                }
                // Now output any actual cuts for the sheet StockNo
                CutsCount = aCalculator.GetStockCutCount(StockNo);
                for (iCut = 0; iCut < CutsCount; iCut++)
                {
                    aCalculator.GetStockCut(StockNo, iCut, out X1, out Y1, out X2, out Y2);
                    Console.Write("Cut={0}:  X1={1};  Y1={2};  X2={3};  Y2={4}\n", iCut, X1, Y1, X2, Y2);
                }
            }
        }
    }
}
