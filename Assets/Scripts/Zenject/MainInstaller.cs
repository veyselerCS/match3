using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<SwipeStartSignal>();
        Container.DeclareSignal<SwipeEndSignal>();
        Container.DeclareSignal<BoardElementPopSignal>();
        Container.DeclareSignal<BoardElementFallSignal>();
        Container.DeclareSignal<BoardElementSpawnedSignal>();
    }
}