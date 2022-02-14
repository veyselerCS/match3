using UnityEngine;
using Zenject;

public class PuzzleInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.DeclareSignal<SwipeStartSignal>();
        Container.DeclareSignal<SwipeEndSignal>();
        Container.DeclareSignal<SwipeFailSignal>();
        Container.DeclareSignal<SpawnEndSignal>();
        Container.DeclareSignal<MatchEndSignal>();
        Container.DeclareSignal<FallEndSignal>();
        Container.DeclareSignal<TapSignal>();
        Container.DeclareSignal<TriggerSignal>();
    }
}