using UnityInitializer.Core.Interfaces;
using UnityInitializer.Core.Models;
using System.IO.Compression;
using System.Reflection;

namespace UnityInitializer.Core.Services
{
    public class UnityProjectInitializer : IUnityProjectInitializer
    {
        public const string DOMAIN_PATH = "UnityInitializer.Core.Source.";

        private readonly Assembly _assembly;

        public UnityProjectInitializer()
        {
            _assembly = Assembly.GetExecutingAssembly();
        }

        public async Task<InitializationResult> InitializeProjectAsync(string targetDirectory, string unityVersion)
        {
            try
            {
                var resourceName = $"{DOMAIN_PATH}{unityVersion}.zip";

                using var stream = _assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    var similarVersions = await FindSimilarVersionsAsync(unityVersion);
                    var suggestions = similarVersions.Any()
                        ? $" Available versions: {string.Join(", ", similarVersions.Take(3))}"
                        : "";

                    return new InitializationResult
                    {
                        Success = false,
                        Message = $"Unity version {unityVersion} not found.{suggestions}"
                    };
                }

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
                zipArchive.ExtractToDirectory(targetDirectory, overwriteFiles: true);

                return new InitializationResult
                {
                    Success = true,
                    Message = $"Unity project {unityVersion} successfully initialized in {targetDirectory}",
                    ActualVersion = unityVersion
                };
            }
            catch (Exception ex)
            {
                return new InitializationResult
                {
                    Success = false,
                    Message = $"Error during project initialization: {ex.Message}"
                };
            }
        }

        public async Task<IEnumerable<string>> GetAvailableVersionsAsync()
        {
            return await Task.Run(() =>
            {
                var resourceNames = _assembly.GetManifestResourceNames()
                    .Where(name => name.StartsWith(DOMAIN_PATH) && name.EndsWith(".zip"))
                    .Select(name => name.Replace(DOMAIN_PATH, "").Replace(".zip", ""))
                    .OrderByDescending(version => version)
                    .ToList();

                return resourceNames.AsEnumerable();
            });
        }

        public async Task<IEnumerable<string>> FindSimilarVersionsAsync(string partialVersion)
        {
            var allVersions = await GetAvailableVersionsAsync();

            return allVersions
                .Where(version => version.StartsWith(partialVersion, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(version => version)
                .ToList();
        }
    }
}