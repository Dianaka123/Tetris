using strange.extensions.context.api;

namespace UnityAcademy.TreeOfControllersExample
{
    public interface IGameRoot
    {
        IRootForGameObjects RootForGameObjects { get; }
        ISpawnManager Spawner { get; }
        IGridManager GridManager { get; }
    }
}