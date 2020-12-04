using Game.Core.Interfaces;
using Game.Gameplay.Interfaces;
using strange.extensions.context.api;

namespace Game.Core
{
    public interface IGameRoot
    {
        IRootForGameObjects RootForGameObjects { get; }
        ISpawnManager Spawner { get; }
        IGridManager GridManager { get; }
        IShapeLoader ShapeLoader { get; }
        TickManager TickManager { get; }
        ISoundManager SoundManager { get; }
    }
}