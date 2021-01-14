using Game.Core.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainUI.Views.Menu
{
    public class GameOverMenuView : GameMenuView
    {
        [SerializeField] private Button Restart;
        [SerializeField] private Button QuitButton;

        [Inject] public GameStartMenuSignal StartMenuSignal { get; set; }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            Restart.onClick.AddListener(DeactivateMenu);
            QuitButton.onClick.AddListener(Quit);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Restart.onClick.RemoveListener(DeactivateMenu);
        }

        public override void DeactivateMenu()
        {
            base.DeactivateMenu();
            StartMenuSignal.Dispatch();
        }

        private void Quit()
        {
            Application.Quit();
        }
    }
}