using BoardKings.Core.Controllers;
using Game.Core;
using Game.Core.Common;
using Game.Core.Interfaces;
using Game.Core.Managers;
using Game.Gameplay.Interfaces;
using Infra.Controllers.Core;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

#if UNITY_IOS || UNITY_EDITOR
#endif

namespace Game
{
    public partial class GameContext : MVCSContext
    {
        private readonly IGameRoot _gameRootView;
        
        public GameContext(MonoBehaviour monoBehaviour, IGameRoot gameRootView)
            : base(monoBehaviour, ContextStartupFlags.MANUAL_MAPPING)
        {
            _gameRootView = gameRootView;
        }

        public T CreateController<T>() where T : ControllerBase
        {
            var factory = injectionBinder.GetInstance<IControllerFactory>();
            return factory.Create<T>();
        }
        
        protected override void mapBindings()
        {
            base.mapBindings();
            
            injectionBinder.Bind<IControllerFactory>().To<ControllerFactory>().ToSingleton();
            
            injectionBinder.Bind<IRootForGameObjects>().ToValue(_gameRootView.RootForGameObjects);
            
            injectionBinder.Bind<IShapeLoader>().ToValue(_gameRootView.ShapeLoader);
            injectionBinder.Bind<IGridManager>().ToValue(_gameRootView.GridManager);
            injectionBinder.Bind<ISpawnManager>().ToValue(_gameRootView.Spawner);
            injectionBinder.Bind<TickManager>().ToValue(_gameRootView.TickManager);
            injectionBinder.Bind<ISoundManager>().ToValue(_gameRootView.SoundManager);

            injectionBinder.Bind<GameOverSignal>().ToSingleton();
            injectionBinder.Bind<LevelChangedSignal>().ToSingleton();
            injectionBinder.Bind<LineFullSignal>().ToSingleton();
            injectionBinder.Bind<ScoreChangedSignal>().ToSingleton();
            injectionBinder.Bind<ShapeVerticalMoveSignal>().ToSingleton();
            injectionBinder.Bind<ShapeHorizontalMoveSignal>().ToSingleton();
            injectionBinder.Bind<ShapeRotateSignal>().ToSingleton();
            injectionBinder.Bind<LinesCountChangedSignal>().ToSingleton();

            injectionBinder.Bind<IStatisticsManager>().To<StatisticsManager>().ToSingleton();

            injectionBinder.BindSelf<GameRootController>().ToSingleton();

            BindGameplay();
            BindMainUI();
        }

    }
}