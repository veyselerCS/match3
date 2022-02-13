using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class AddressableManager : Manager
{
    public override void Begin()
    {
        SetReady();
    }
    
    public async Task<T> LoadAsset<T>(string label)
    {
        return await Addressables.LoadAssetAsync<T>(label).Task;
    }
}