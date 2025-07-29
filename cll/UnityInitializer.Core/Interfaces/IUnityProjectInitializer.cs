using UnityInitializer.Core.Models;

namespace UnityInitializer.Core.Interfaces
{
    public interface IUnityProjectInitializer
    {
        Task<InitializationResult> InitializeProjectAsync(string targetDirectory, string unityVersion);
        Task<IEnumerable<string>> GetAvailableVersionsAsync();
        Task<IEnumerable<string>> FindSimilarVersionsAsync(string partialVersion);
    }
}