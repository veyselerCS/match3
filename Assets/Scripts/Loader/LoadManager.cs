using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class LoadManager : Manager
{
    [Inject] private SignalBus _signalBus;

    public override void Begin()
    {
        _signalBus.Subscribe<DataReadySignal>(OnDataReadySignal);
        SetReady();
    }

    private void OnDataReadySignal()
    {
        SceneManager.LoadScene("PuzzleScene", LoadSceneMode.Additive);
    }
}