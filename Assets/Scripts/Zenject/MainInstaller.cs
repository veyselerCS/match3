using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<ManagerReadySignal>();
        Container.DeclareSignal<ManagerProviderReadySignal>();
    }
}