using Game.Core.Common;
using Game.MainUI.Views.Menu;
using strange.extensions.mediation.impl;

namespace Game.MainUI.Mediators
{
    public class GameStartMenuMediator : Mediator
    {
        [Inject] public GameStartMenuView GameStartMenuView { get; set; }
        
        [Inject] public GameStartMenuSignal GameStartMenuSignal { get; set; }
        
        public override void OnRegister()
        {
            base.OnRegister();
            GameStartMenuSignal.AddListener(GameStartMenuCallback);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            GameStartMenuSignal.RemoveListener(GameStartMenuCallback);
        }
        
        private void GameStartMenuCallback()
        {
            GameStartMenuView.ActivateMenu();
        }
    }
}