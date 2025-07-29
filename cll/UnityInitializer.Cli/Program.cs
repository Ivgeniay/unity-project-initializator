using UnityInitializer.Core.Interfaces;
using UnityInitializer.Core.Services;
using UnityInitializer.Core.Consts;
using System.CommandLine;


namespace UnityInitializer.Cli
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var versionOption = new Option<string?>(CommandConstants.Options.Version.SHORT_ALIAS, CommandConstants.Options.Version.LONG_ALIAS)
            {
                Description = CommandConstants.Options.Version.DESCRIPTION
            };

            var listOption = new Option<bool>(CommandConstants.Options.List.SHORT_ALIAS, CommandConstants.Options.List.LONG_ALIAS)
            {
                Description = CommandConstants.Options.List.DESCRIPTION
            };

            var rootCommand = new RootCommand(CommandConstants.Messages.ROOT_COMMAND_DESCRIPTION);

            var versionOptionToRemove = rootCommand.Options.FirstOrDefault(o => o.Name == CommandConstants.SystemOptions.VERSION_OPTION_NAME);
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

                var initializer = new UnityProjectInitializer();

                if (list)
                {
                    await HandleListCommand(initializer);
                    return 0;
                }

                if (string.IsNullOrWhiteSpace(version))
                {
                    Console.WriteLine(CommandConstants.Messages.VERSION_REQUIRED_ERROR);
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
            Console.WriteLine(CommandConstants.Messages.AVAILABLE_VERSIONS_HEADER);

            var versions = await initializer.GetAvailableVersionsAsync();

            if (!versions.Any())
            {
                Console.WriteLine(CommandConstants.Messages.NO_VERSIONS_FOUND);
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

            Console.WriteLine(string.Format(CommandConstants.Messages.INITIALIZING_PROJECT, version, currentDirectory));

            var result = await initializer.InitializeProjectAsync(currentDirectory, version);

            if (result.Success)
            {
                Console.WriteLine(result.Message);
            }
            else
            {
                Console.WriteLine(string.Format(CommandConstants.Messages.INITIALIZATION_FAILED, result.Message));
            }
        }


    }

}

