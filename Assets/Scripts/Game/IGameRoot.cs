
using Game.Gameplay.Interfaces;
using Models.Interfaces;
using Models.Managers;
using strange.extensions.context.api;
using UnityAcademy.TreeOfControllersExample;

namespace Game
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