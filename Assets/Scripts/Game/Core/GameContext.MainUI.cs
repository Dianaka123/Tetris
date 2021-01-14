using Game.Core.Common;
using Game.MainUI;
using Game.MainUI.Mediators;
using Game.MainUI.Views.Menu;
using Game.MainUI.Views.Views;
using UnityAcademy.TreeOfControllersExample.Views;

namespace Game
{
    public partial class GameContext
    {
        private void BindMainUI()
        {
            mediationBinder.Bind<ScoreHudView>().To<HUDMediator>();
            mediationBinder.Bind<LevelHudView>().To<LevelMediator>();
            mediationBinder.Bind<LinesHudView>().To<LineCountMediator>();
            mediationBinder.Bind<GameOverMenuView>().To<GameOverMediator>();
            mediationBinder.Bind<GameStartMenuView>().To<GameStartMenuMediator>();
            
            injectionBinder.BindSelf<MainUIController>().ToSingleton();
        }
    }
}