using LispParser.Contracts;
using LispParser.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LispParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var serviceProvider = new ServiceCollection()
                .AddScoped<IConsole, ConsoleProvider>()
                .AddScoped<IFileService, FileService>()
                .AddScoped<IParserService, ParserService>()
                .AddScoped<IRunService, RunService>()
                .AddScoped<IUserFeedbackService, UserFeedbackService>()
                .BuildServiceProvider();

            var runService = serviceProvider.GetService<IRunService>();

            var program = new RunProgram(runService);

            try
            {
                await program.RunAsync(args, new CancellationToken());
            }
            catch (ArgumentException ex)
            {
                stopwatch.Stop();
                Console.WriteLine("The file is invalid: " + ex.Message);
                Console.WriteLine("Time Elapsed: " + stopwatch.ElapsedMilliseconds);
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine("An unknown exception occured: " + ex.Message);
                Console.WriteLine("Time Elapsed: " + stopwatch.ElapsedMilliseconds);
                Environment.Exit(1);
            }

            stopwatch.Stop();
            Console.WriteLine("File is valid. Time Elapsed: " + stopwatch.ElapsedMilliseconds);
            Environment.Exit(0);
        }
    }
}
