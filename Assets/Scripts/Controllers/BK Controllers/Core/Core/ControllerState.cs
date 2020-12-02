namespace Infra.Controllers.Core
{
    public enum ControllerState
    {
        Created,
        Initialized,
        Running,
        ChildsStopped,
        Stopped,
        Disposed
    }
}