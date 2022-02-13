using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<ManagerReadySignal>().OptionalSubscriber();
        Container.DeclareSignal<ManagerProviderReadySignal>();
        Container.DeclareSignal<DataReadySignal>();
    }
}