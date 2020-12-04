
using Game;
using Game.Core.Common;
using Game.MainUI.Views.Menu;
using strange.extensions.mediation.impl;

namespace UnityAcademy.TreeOfControllersExample.Views
{
    public class GameOverMediator : Mediator
    {
        [Inject]
        public GameOverMenuView GameMenuView { get; set; }
        
        [Inject] 
        public GameOverSignal GameOverSignal { get; set; }
        
        public override void OnRegister()
        {
            base.OnRegister();
            GameOverSignal.AddListener(GameOverCallback);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            GameOverSignal.RemoveListener(GameOverCallback);
        }
        
        private void GameOverCallback()
        {
            GameMenuView.ActivateMenu();
        }
    }
}