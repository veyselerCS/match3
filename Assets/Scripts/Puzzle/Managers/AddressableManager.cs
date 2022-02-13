using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : Manager
{
    public override void Begin()
    {
        SetReady();
    }
    
    public AsyncOperationHandle<T> LoadAsset<T>(string label)
    {
        return Addressables.LoadAssetAsync<T>(label);
    }
}