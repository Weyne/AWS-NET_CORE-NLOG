using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using WeyneLoggingWithNLog.Interfaces;
using WeyneLoggingWithNLog.Services;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace WeyneLoggingWithNLog
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            try
            {

                var builder = new ConfigurationBuilder();
                BuildConfig(builder);

                var host = Host.CreateDefaultBuilder()
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.ClearProviders(); //esta línea hace la diferencia
                        logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace); //esta línea hace la diferencia
                        logging.AddNLogWeb();
                    })
                    .ConfigureServices((context, services) =>
                    {
                        services.AddHttpContextAccessor();
                        services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
                        services.AddScoped<IProcessService, ProcessService>();
                        services.BuildServiceProvider();
                        services.AddLogging();
                    })
                    .UseNLog()
                    .Build();

                var service = ActivatorUtilities.CreateInstance<ProcessService>(host.Services);

                return service.Invoke(input).Result.ToUpper();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw ex;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }    
}
