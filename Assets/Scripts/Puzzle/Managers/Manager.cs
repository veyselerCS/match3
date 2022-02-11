using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public abstract class Manager : MonoBehaviour
{
    protected ManagerProvider _managerProvider;
    protected List<Manager> _dependencies = new List<Manager>();

    [Inject] private SignalBus _signalBus;

    private void Awake()
    {
        _managerProvider = ManagerProvider.Instance;
        _managerProvider.Register(this);
    }

    private bool _init = false;
    private void Start()
    {
        Init();
        for (int i = _dependencies.Count - 1; i >= 0; i--)
        {
            var dependency = _dependencies[i];
            if (_managerProvider.IsResolved(dependency))
                _dependencies.Remove(dependency);
        }

        if (_dependencies.Count == 0)
        {
            _signalBus.Subscribe<ManagerReadySignal>(OnManagerReady);
            Resolve();
            return;
        }
        
        _signalBus.Subscribe<ManagerReadySignal>(OnManagerReady);
    }

    //create dependencies
    //if no dependency resolve here
    public virtual void Init()
    {
        
    }

    //ready the manager here after the operations
    public abstract void Begin();

    public virtual void Resolve()
    {
        _signalBus.Unsubscribe<ManagerReadySignal>(OnManagerReady);
        Begin();
    }

    protected void SetReady()
    {
        Debug.LogWarning("Resolved" + this.name);
        _managerProvider.AddResolved(this);
        _signalBus.Fire(new ManagerReadySignal(this));
    }

    public virtual void OnManagerReady(ManagerReadySignal data)
    {
        if (_dependencies.Contains(data.Manager))
        {
            _dependencies.Remove(data.Manager);
        }

        if (_dependencies.Count == 0) Resolve();
    }
}