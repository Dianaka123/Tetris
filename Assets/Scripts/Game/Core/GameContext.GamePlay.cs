using Game.Gameplay;
using Game.Gameplay.Interfaces;
using Game.Gameplay.Models;

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