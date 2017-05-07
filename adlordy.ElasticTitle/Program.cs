using adlordy.ElasticTitle.Contracts;
using adlordy.ElasticTitle.Extensions;
using adlordy.ElasticTitle.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using adlordy.ElasticTitle.Models;

namespace adlordy.ElasticTitle
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var token = new CancellationTokenSource())
            {
                var serviceProvider = new ServiceCollection()
                    .AddLogging()
                    .AddElastic()
                    .AddScoped<Processor>()
                    .AddTransient<ITitleParser,TitleParser>()
                    .BuildServiceProvider();

                var factory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var logger = factory.AddConsole(Microsoft.Extensions.Logging.LogLevel.Debug)
                    .CreateLogger<Program>();

                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    Console.CancelKeyPress += (sender, eventArgs) =>
                    {
                        token.Cancel();
                        // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                        eventArgs.Cancel = true;
                    };

                    logger.LogInformation("Started Elastic Title");
                    var watch = new Stopwatch();
                    watch.Start();
                    var processor = ActivatorUtilities.CreateInstance<Processor>(scope.ServiceProvider, token.Token);
                    processor.Process();
                    watch.Stop();
                    //token.Token.WaitHandle.WaitOne();
                    logger.LogInformation("Stopping Elastic Title. Time: {0}", watch.Elapsed);
                }

            }
        }

        
    }
}