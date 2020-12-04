using BoardKings.Core.Controllers;
using Game.Gameplay;
using Game.Gameplay.Interfaces;
using Game.Gameplay.Models;
using Infra.Controllers.Core;
using Models.Interfaces;
using strange.extensions.context.api;
using UnityAcademy.TreeOfControllersExample;

namespace Game
{
    public partial class GameContext
    {
        private void BindGameplay()
        {
            injectionBinder.Bind<IShapeManager>().To<ShapeManager>().ToSingleton();
            injectionBinder.Bind<IShapeRandomizer>().To<ShapeRandomizer>().ToSingleton();

            injectionBinder.BindSelf<GameplayController>().ToSingleton();
        }

        public void UnbindCrossContextGameplay()
        {
            
        }
    }
}