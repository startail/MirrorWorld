using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Infrastructure.Services.Interfaces
{
    public interface IAssetLoader
    {
        UniTask<T> LoadAssetAsync<T>(string key, CancellationToken cancellationToken = default);
        UniTask<T> LoadAssetAsync<T>(AssetReference reference, CancellationToken cancellationToken = default);
    }
}
