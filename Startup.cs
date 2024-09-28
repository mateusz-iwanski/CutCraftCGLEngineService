using CutCraftEngineData.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.IO;
using CutCraftCGLEngineService.CGLCalculator;
using CutCraftEngineData.DataInput;

namespace CutCraftCGLEngineService
{

    /// <summary>
    /// Represents a builder for configuring services.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the ConfigureServices class.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>((serviceProvider) =>
            {
                return new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("settings.json", optional: false, reloadOnChange: true)
                    .Build();
            });

            // Add NLog as the logging provider
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders(); // Clear any existing logging providers
                loggingBuilder.SetMinimumLevel(LogLevel.Trace); // Set the minimum log level

                // Add NLog
                loggingBuilder.AddNLog();
            });
            // Register NLog's ILogger
            services.AddSingleton<NLog.ILogger>(provider => NLog.LogManager.GetCurrentClassLogger());

            services.AddScoped<CGLCalculator.CGLCalculator>();            

            // nopCommerce customer services
            services.AddScoped<Func<string, ICalculator>>(serviceProvider => key =>
            {
                return key switch
                {
                    "cutglib" => serviceProvider.GetService<CGLCalculator.CGLCalculator>() as ICalculator,
                    _ => throw new Exceptions.ArgumentException($"Unknown key: {key}")
                };
            });
        }
    }
}
