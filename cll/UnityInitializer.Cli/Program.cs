using UnityInitializer.Core.Interfaces;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using UnityInitializer.Core.Services;


namespace UnityInitializer.Cli
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var versionOption = new Option<string?>("-v", "--version")
            {
                Description = "Unity version to initialize project with"
            };

            var listOption = new Option<bool>("-l", "--list")
            {
                Description = "List all available Unity versions"
            };

            var rootCommand = new RootCommand("Unity project initializer");

            var versionOptionToRemove = rootCommand.Options.FirstOrDefault(o => o.Name == "--version");
            if (versionOptionToRemove != null)
            {
                rootCommand.Options.Remove(versionOptionToRemove);
            }

            rootCommand.Add(versionOption);
            rootCommand.Add(listOption);

            rootCommand.SetAction(async (parseResult, cancellationToken) =>
            {
                var version = parseResult.GetValue(versionOption);
                var list = parseResult.GetValue(listOption);

                Console.WriteLine($"DEBUG: version='{version}', list={list}");

                var initializer = new UnityProjectInitializer();

                if (list)
                {
                    await HandleListCommand(initializer);
                    return 0;
                }

                if (string.IsNullOrWhiteSpace(version))
                {
                    Console.WriteLine("Error: Version parameter is required. Use -v to specify Unity version or -l to list available versions.");
                    return 1;
                }

                await HandleInitCommand(initializer, version);
                return 0;
            });

            var parseResult = rootCommand.Parse(args);
            return await parseResult.InvokeAsync();
        }

        private static async Task HandleListCommand(IUnityProjectInitializer initializer)
        {
            Console.WriteLine("Available Unity versions:");

            var versions = await initializer.GetAvailableVersionsAsync();

            if (!versions.Any())
            {
                Console.WriteLine("No Unity versions found in embedded resources.");
                return;
            }

            foreach (var version in versions)
            {
                Console.WriteLine($"  {version}");
            }
        }

        private static async Task HandleInitCommand(IUnityProjectInitializer initializer, string version)
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            Console.WriteLine($"Initializing Unity project version {version} in {currentDirectory}...");

            var result = await initializer.InitializeProjectAsync(currentDirectory, version);

            if (result.Success)
            {
                Console.WriteLine(result.Message);
            }
            else
            {
                Console.WriteLine($"Failed: {result.Message}");
            }
        }


    }

}

