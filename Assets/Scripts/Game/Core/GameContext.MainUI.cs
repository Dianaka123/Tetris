using Game.MainUI;
using Game.MainUI.Mediators;
using Game.MainUI.Views.Views;

namespace Game
{
    public partial class GameContext
    {
        private void BindMainUI()
        {
            mediationBinder.Bind<ScoreHudView>().To<HUDMediator>();
            mediationBinder.Bind<LevelHudView>().To<LevelMediator>();
            mediationBinder.Bind<LinesHudView>().To<LineCountMediator>();

            injectionBinder.BindSelf<MainUIController>().ToSingleton();
        }
    }
}