using UnityEngine;
using Zenject;

public class PuzzleInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.DeclareSignal<SwipeStartSignal>();
        Container.DeclareSignal<SwipeEndSignal>();
        Container.DeclareSignal<MatchEndSignal>();
        Container.DeclareSignal<SpawnEndSignal>();
        Container.DeclareSignal<FallEndSignal>();
        Container.DeclareSignal<TapSignal>();
        Container.DeclareSignal<TriggerSignal>();
    }
}