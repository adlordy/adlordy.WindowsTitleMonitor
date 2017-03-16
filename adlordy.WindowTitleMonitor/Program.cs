using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using adlordy.WindowTitleMonitor.Contracts;
using System.Threading.Tasks;
using adlordy.WindowTitleMonitor.Services;
using adlordy.Outlook.Contracts;
using adlordy.Outlook.Services;

namespace adlordy.WindowTitleMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var token = new CancellationTokenSource())
            {
                var serviceProvider = new ServiceCollection()
                    .AddLogging()
                    .AddScoped<ITitleService, TitleService>()
                    .AddScoped<IWriter, FileWriter>()
                    .AddScoped<IReportBuilder, ReportBuilder>()
                    .AddScoped<IPathProvider, PathProvider>()
                    .AddScoped<ISubjectReader,SubjectReader>()
                    .BuildServiceProvider();

                var factory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var logger = factory.AddConsole(LogLevel.Debug).CreateLogger<Program>();

                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    Console.CancelKeyPress += (sender, eventArgs) =>
                    {
                        token.Cancel();
                        // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                        eventArgs.Cancel = true;
                    };

                    logger.LogInformation("Started title monitor");
                    MainLoop(token, scope);
                    token.Token.WaitHandle.WaitOne();
                    logger.LogInformation("Stopping title monitor");
                }

                GenerateReport(logger,serviceProvider);
            }
        }

        private static void MainLoop(CancellationTokenSource token, IServiceScope scope)
        {
            var writer = scope.ServiceProvider.GetRequiredService<IWriter>();
            var titleService = scope.ServiceProvider.GetRequiredService<ITitleService>();
            var report = scope.ServiceProvider.GetRequiredService<IReportBuilder>();
            var subjectReader = scope.ServiceProvider.GetRequiredService<ISubjectReader>();

            Task.Run(async () =>
            {
                while (!token.Token.IsCancellationRequested)
                {
                    var date = DateTime.Now;

                    var title = titleService.GetWindowTitle();
                    if (title.EndsWith(" - Outlook"))
                        title = subjectReader.GetOutlookSubject()+ " - Outlook";
                    writer.Write(date, title);

                    await Task.Delay(1000);

                    if (date.Date != DateTime.Today)
                        report.Build(date);
                }
            });
        }

        private static void GenerateReport(ILogger<Program> logger, IServiceProvider serviceProvider)
        {
            logger.LogInformation("Generating report");
            var report = serviceProvider.GetRequiredService<IReportBuilder>();
            report.Build(DateTime.Today);
        }
    }
}
