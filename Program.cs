﻿using CutGLib;
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
using CutCraftCGLEngineService.CGLCalculator;
using CutCraftCGLEngineService.DataOutput;
using CutCraftEngineData.DataOutput;

namespace CutCraftCGLEngineService
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

            ICalculator calculator = scope.ServiceProvider.GetRequiredService<Func<string, ICalculator>>()(cutterEngine.ToString());

            if (calculator.GetType() == typeof(CGLCalculator.CGLCalculator))
            {
                CGLDisplayLogger cGLConsoleLogger = new CGLDisplayLogger((CGLCalculator.CGLCalculator)calculator, Console.WriteLine);
            }

            // Deserialize the JSON file into a Command object
            string json = File.ReadAllText("test_input.json");
            Command command = JsonConvert.DeserializeObject<Command>(json);
            calculator.Execute(command);
            Console.WriteLine("================" + calculator.GetDataCuttingsOutputs());

            // generate and print data output in standard
            CutEngine cutEngine = ((CGLCalculator.CGLCalculator)calculator).GetCutEngine();
            CGLDataOutputFactory cGLDataOutputFactory = new CGLDataOutputFactory(cutEngine, command.Input, calculator.GetDataCuttingsOutputs());
            List<DataOutputs> dataOutputs = cGLDataOutputFactory.Get();
            Console.WriteLine(JsonConvert.SerializeObject(dataOutputs));
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            // Handle errorss
            Console.WriteLine("Error parsing arguments.");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
               .ConfigureServices((_, services) =>
               {
                   var startup = new Startup();
                   startup.ConfigureServices(services);
               });
    }
}
