using System;
using Autofac.Extensions.DependencyInjection;
using AutomationFoundation.Hosting;
using AutomationFoundation.Hosting.Abstractions;
using ConsoleRunner.Abstractions;
using ConsoleRunner.Infrastructure.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleRunner
{
    public class Program
    {
        private readonly IConsoleWriter output;
        private readonly IConsoleReader input;
        private readonly IRuntimeHost host;

        private Program(IConsoleReader input, IConsoleWriter output, IRuntimeHost host)
        {
            this.input = input ?? throw new ArgumentNullException(nameof(input));
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            this.host = host ?? throw new ArgumentNullException(nameof(host));
        }

        private static IRuntimeHost CreateRuntimeHost()
        {
            return RuntimeHost.CreateBuilder<DefaultRuntimeHostBuilder>()
                .ConfigureServices(services =>
                {
                    services.AddAutofac();
                    services.AddLogging(logging => logging
                        .AddConsole()
                        .SetMinimumLevel(LogLevel.Information));
                })
                .UseStartup<Startup>()
                .Build();
        }

        public static void Main()
        {
            var reader = new ConsoleReader();
            var writer = new ConsoleWriter();

            try
            {
                var runtime = CreateRuntimeHost();

                var p = new Program(reader, writer, runtime);
                p.Run();
            }
            catch (Exception ex)
            {
                writer.WriteLine(ex.ToString(), ConsoleColor.Red);
            }
            finally
            {
                writer.WriteLine("Press any key to terminate...");
                reader.WaitForAnyKey();
            }
        }

        private void Run()
        {
            host.Start();

            output.WriteLine("Press any key to stop...");
            input.WaitForAnyKey();

            host.Stop();
        }
    }
}