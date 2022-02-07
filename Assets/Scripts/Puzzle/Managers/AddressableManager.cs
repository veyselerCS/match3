using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
    public static AddressableManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public AsyncOperationHandle LoadAsset<T>(string label)
    {
        return Addressables.LoadAssetAsync<T>(label);
    }
}