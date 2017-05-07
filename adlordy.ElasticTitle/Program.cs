using adlordy.ElasticTitle.Contracts;
using adlordy.ElasticTitle.Extensions;
using adlordy.ElasticTitle.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Diagnostics;

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
                var logger = factory.AddConsole(LogLevel.Debug)
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
                    try
                    {
                        processor.Process();
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch(Exception ex)
                    {
                        logger.LogError(new EventId(), ex, "Error occured during Elastic Title");
                    }
                    watch.Stop();
                    logger.LogInformation("Stopped Elastic Title. Time: {0}", watch.Elapsed);
                }

            }
        }

        
    }
}