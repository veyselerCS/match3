using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerProvider : MonoBehaviour
{
    public static ManagerProvider Instance;
    private Dictionary<string, Manager> Managers = new Dictionary<string, Manager>();
    private List<Manager> _resolved = new List<Manager>();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive).completed += (a) =>
        {
            DOVirtual.DelayedCall(2f, () =>
            {
                LoadingManager.Instance.HideLoadingImage();
            });
        };
    }

    public void AddResolved(Manager manager)
    {
        _resolved.Add(manager);
    }
    
    public void Register<T>(T manager) where T : Manager
    {
        var name = manager.GetType().Name;
        if (Managers.ContainsKey(name))
        {
            Debug.LogWarning("Manager with name " +name+" name already added under " + Managers[name].gameObject.name + " game object.");
            Application.Quit();
            return;
        }

        Managers[name] = manager;
    }   
    
    public void UnRegister<T>(T manager) where T : Manager
    {
        var name = manager.GetType().Name;

        Managers.Remove(name);
        _resolved.Remove(manager);
    }
    

    public bool IsResolved(Manager manager)
    {
        return _resolved.Contains(manager);
    }
    
    public T Get<T>() where T : Manager
    {
        if(!Managers.ContainsKey(typeof(T).Name)) Debug.LogWarning("Cant locate : " + typeof(T).Name);
        return (T) Managers[typeof(T).Name];
    } 
}