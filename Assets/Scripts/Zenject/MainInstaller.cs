using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<SwipeStartSignal>();
        Container.DeclareSignal<SwipeEndSignal>();
        Container.DeclareSignal<MatchEndSignal>();
        Container.DeclareSignal<SpawnEndSignal>();
        Container.DeclareSignal<FallEndSignal>();
    }
}