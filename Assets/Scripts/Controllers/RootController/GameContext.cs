using BoardKings.Core.Controllers;
using Controllers;
using Infra.Controllers.Core;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityAcademy.TreeOfControllersExample;
using UnityEngine;
#if UNITY_IOS || UNITY_EDITOR
#endif

namespace UnityAcademy.TreeOfControllersExample
{
    public class GameContext : MVCSContext
    {
        private readonly IGameRoot _gameRootView;
        
        public GameContext(IGameRoot gameRootView)
            :
            base(gameRootView as MonoBehaviour, ContextStartupFlags.MANUAL_MAPPING)
        {
            _gameRootView = gameRootView;
        }

        public T CreateController<T>() where T : ControllerBase
        {
            var factory = injectionBinder.GetInstance<IControllerFactory>();
            return factory.Create<T>();
        }

        protected override void addCoreComponents()
        {
            base.addCoreComponents();

            // injectionBinder.Unbind<ICommandBinder>();
        }

        protected override void mapBindings()
        {
            base.mapBindings();

            injectionBinder.Bind<IRootForGameObjects>().ToValue(_gameRootView.RootForGameObjects);
            injectionBinder.Bind<ISpawnManager>().ToValue(_gameRootView.Spawner);
            injectionBinder.Bind<IGridManager>().ToValue(_gameRootView.GridManager);

            injectionBinder.Bind<IControllerFactory>().To<ControllerFactory>().ToSingleton();
            injectionBinder.Bind<GameRootController>().To<GameRootController>().ToSingleton();
            injectionBinder.BindSelf<GameController>();
        }


        protected override void postBindings()
        {
        }


        private void BindCrossContextEntities()
        {
        }

        private void UnbindCrossContextEntities()
        {
        }

        public override void Dispose()
        {
            UnbindCrossContextEntities();

            base.Dispose();
        }
    }
}