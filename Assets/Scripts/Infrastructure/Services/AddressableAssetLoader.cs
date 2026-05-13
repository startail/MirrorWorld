using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Infrastructure.Services.Interfaces;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Infrastructure.Services
{
    public class AddressableAssetLoader : IAssetLoader, IDisposable
    {
        private readonly List<AsyncOperationHandle> _handles = new List<AsyncOperationHandle>();
        private bool _disposed;

        public async UniTask<T> LoadAssetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AddressableAssetLoader));

            var handle = Addressables.LoadAssetAsync<T>(key);
            _handles.Add(handle);

            try
            {
                return await handle.ToUniTask(cancellationToken: cancellationToken);
            }
            catch
            {
                _handles.Remove(handle);
                Addressables.Release(handle);
                throw;
            }
        }

        public async UniTask<T> LoadAssetAsync<T>(AssetReference reference, CancellationToken cancellationToken = default)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AddressableAssetLoader));

            var handle = Addressables.LoadAssetAsync<T>(reference);
            _handles.Add(handle);

            try
            {
                return await handle.ToUniTask(cancellationToken: cancellationToken);
            }
            catch
            {
                _handles.Remove(handle);
                Addressables.Release(handle);
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            foreach (var handle in _handles)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            _handles.Clear();
            _disposed = true;
        }
    }
}
